using AutoMapper;
using Mok.Blog.Models;
using Mok.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mok.Blog.Helpers
{
    public class BlogUtil
    {
        /// <summary>
        /// Returns a valid slug
        /// </summary>
        /// <remarks>
        /// This method makes sure the result slug
        /// - not to exceed max len;
        /// - if <see cref="Util.Slugify(string)"/> returns empty string, it generates a random one;
        /// - a unique value if its a duplicate with existings slugs;
        /// - if '#' char is present, I swap it to 's'
        /// </remarks>
        public static string SlugifyTaxonomy(string title, int maxlen, IEnumerable<string> existingSlugs = null)
        {
            // preserve # as s before format to slug
            title = title.Replace('#', 's');

            // make slug
            var slug = Util.Slugify(title, maxlen: maxlen, randomCharCountOnEmpty: 6);

            // make sure slug is unique
            slug = Util.UniquefySlug(slug, existingSlugs);

            return slug;
        }

        /// <summary>
        /// Returns automapper mapping.
        /// </summary>
        /// <remarks>
        /// https://github.com/AutoMapper/AutoMapper/issues/1441
        /// </remarks>
        public static IMapper Mapper
        {
            get
            {
                return new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Post, BlogPost>();
                    cfg.CreateMap<BlogPost, Post>();
                }).CreateMapper();
            }
        }
    }
}
