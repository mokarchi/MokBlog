using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Mok.Blog.Data;
using Mok.Blog.Events;
using Mok.Blog.Helpers;
using Mok.Blog.Models;
using Mok.Blog.Services.Interfaces;
using Mok.Exceptions;
using Mok.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mok.Blog.Services
{
    /// <summary>
    /// Unit tests for <see cref="TagService"/>.
    /// </summary>
    public class TagService : ITagService,
                               INotificationHandler<BlogPostBeforeCreate>
    {
        private readonly ITagRepository tagRepository;
        private readonly IDistributedCache cache;
        private readonly ILogger<TagService> logger;

        public TagService(ITagRepository tagRepository,
                          IDistributedCache cache,
                          ILogger<TagService> logger)
        {
            this.tagRepository = tagRepository;
            this.cache = cache;
            this.logger = logger;
        }

        /// <summary>
        /// The max allowed length of a tag title is 24 chars.
        /// </summary>
        public const int TITLE_MAXLEN = 24;

        /// <summary>
        /// The max allowed length of a tag slug is 24 chars.
        /// </summary>
        public const int SLUG_MAXLEN = 24;

        /// <summary>
        /// Returns all tags, cached after calls to DAL.
        /// </summary>
        public async Task<List<Tag>> GetAllAsync()
        {
            return await cache.GetAsync(BlogCache.KEY_ALL_TAGS, BlogCache.Time_AllTags, async () => {
                return await tagRepository.GetListAsync();
            });
        }

        /// <summary>
        /// Creates a new <see cref="Tag"/>.
        /// </summary>
        /// <param name="tag">The tag with data to be created.</param>
        /// <exception cref="MokException">If tag is empty or title exists already.</exception>
        /// <returns>Created tag.</returns>
        public async Task<Tag> CreateAsync(Tag tag)
        {
            if (tag == null || tag.Title.IsNullOrEmpty())
            {
                throw new MokException($"Invalid tag to create.");
            }

            // prep title
            tag.Title = PrepareTitle(tag.Title);

            // make sure it is unique
            var allTags = await GetAllAsync();
            if (allTags.Any(t => t.Title.Equals(tag.Title, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new MokException($"'{tag.Title}' already exists.");
            }

            // prep slug, description and count
            tag.Slug = BlogUtil.SlugifyTaxonomy(tag.Title, SLUG_MAXLEN, allTags.Select(c => c.Slug)); // slug is based on title
            tag.Description = Util.CleanHtml(tag.Description);
            tag.Count = tag.Count;

            // create
            tag = await tagRepository.CreateAsync(tag);

            // remove cache
            await cache.RemoveAsync(BlogCache.KEY_ALL_TAGS);

            logger.LogDebug("Created {@Tag}", tag);
            return tag;
        }

        /// <summary>
        /// Updates an existing <see cref="Tag"/>.
        /// </summary>
        /// <param name="tag">The tag with data to be updated.</param>
        /// <exception cref="MokException">If tag is invalid or title exists already.</exception>
        /// <returns>Updated tag.</returns>
        public async Task<Tag> UpdateAsync(Tag tag)
        {
            if (tag == null || tag.Id <= 0 || tag.Title.IsNullOrEmpty())
            {
                throw new MokException($"Invalid tag to update.");
            }

            // prep title
            tag.Title = PrepareTitle(tag.Title);

            // make sure it is unique
            var allTags = await GetAllAsync();
            allTags.RemoveAll(t => t.Id == tag.Id); // remove self
            if (allTags.Any(t => t.Title.Equals(tag.Title, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new MokException($"'{tag.Title}' already exists.");
            }

            // prep slug, description and count
            var entity = await tagRepository.GetAsync(tag.Id);
            entity.Title = tag.Title; // assign new title
            entity.Slug = BlogUtil.SlugifyTaxonomy(tag.Title, SLUG_MAXLEN, allTags.Select(c => c.Slug)); // slug is based on title
            entity.Description = Util.CleanHtml(tag.Description);
            entity.Count = tag.Count;

            // update 
            await tagRepository.UpdateAsync(entity);

            // remove cache
            await cache.RemoveAsync(BlogCache.KEY_ALL_TAGS);

            // return entity
            logger.LogDebug("Updated {@Tag}", entity);
            return entity;
        }

        /// <summary>
        /// Deletes a <see cref="Tag"/> by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            await tagRepository.DeleteAsync(id);
            await cache.RemoveAsync(BlogCache.KEY_ALL_TAGS);
        }

        /// <summary>
        /// Returns tag by id, throws <see cref="MokException"/> if tag with id is not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Tag> GetAsync(int id)
        {
            var tags = await GetAllAsync();
            var tag = tags.SingleOrDefault(c => c.Id == id);
            if (tag == null)
            {
                throw new MokException(EExceptionType.ResourceNotFound,
                    $"Tag with id {id} is not found.");
            }

            return tag;
        }

        /// <summary>
        /// Returns tag by slug, throws <see cref="MokException"/> if tag with slug is not found.
        /// </summary>
        /// <param name="slug">Tag slug.</param>
        /// <returns></returns>
        public async Task<Tag> GetBySlugAsync(string slug)
        {
            if (slug.IsNullOrEmpty())
                throw new MokException(EExceptionType.ResourceNotFound, "Tag does not exist.");

            var tags = await GetAllAsync();
            var tag = tags.SingleOrDefault(c => c.Slug.Equals(slug, StringComparison.CurrentCultureIgnoreCase));
            if (tag == null)
            {
                throw new MokException(EExceptionType.ResourceNotFound, $"Tag '{slug}' does not exist.");
            }

            return tag;
        }

        /// <summary>
        /// Cleans tag title from any html and shortens it if exceed max allow length.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string PrepareTitle(string title)
        {
            title = Util.CleanHtml(title);
            title = title.Length > TITLE_MAXLEN ? title.Substring(0, TITLE_MAXLEN) : title;
            return title;
        }

        /// <summary>
        /// Handles the <see cref="BlogPostBeforeCreate"/> event by creating any new tags.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(BlogPostBeforeCreate notification, CancellationToken cancellationToken)
        {
            if (notification.TagTitles == null || notification.TagTitles.Count <= 0) return;

            // make sure list has no empty strings and only unique values
            var distinctTitles = notification.TagTitles.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();
            var allTags = await GetAllAsync();

            // create any new tags
            foreach (var title in distinctTitles)
            {
                // make sure the incoming title does not already exist
                var tag = allTags.FirstOrDefault(t => t.Title.Equals(title, StringComparison.CurrentCultureIgnoreCase));
                if (tag == null)
                {
                    tag = await CreateAsync(new Tag { Title = title });
                }
            }
        }
    }
}
