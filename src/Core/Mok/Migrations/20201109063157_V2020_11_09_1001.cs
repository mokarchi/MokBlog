using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mok.Migrations
{
    public partial class V2020_11_09_1001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blog_Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Slug = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog_Category", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Blog_Tag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Slug = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog_Tag", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Core_Meta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Meta", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Blog_Post",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Body = table.Column<string>(nullable: true),
                    BodyMark = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    CommentCount = table.Column<int>(nullable: false),
                    CommentStatus = table.Column<byte>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    Excerpt = table.Column<string>(nullable: true),
                    ParentId = table.Column<int>(nullable: true),
                    RootId = table.Column<int>(nullable: true),
                    Slug = table.Column<string>(maxLength: 256, nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    Title = table.Column<string>(maxLength: 256, nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ViewCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blog_Post_Blog_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Blog_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Category_Slug",
                table: "Blog_Category",
                column: "Slug",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Post_CategoryId",
                table: "Blog_Post",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Post_ParentId",
                table: "Blog_Post",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Post_Slug",
                table: "Blog_Post",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Post_Type_Status_CreatedOn_Id",
                table: "Blog_Post",
                columns: new[] { "Type", "Status", "CreatedOn", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Tag_Slug",
                table: "Blog_Tag",
                column: "Slug",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Core_Meta_Type_Key",
                table: "Core_Meta",
                columns: new[] { "Type", "Key" },
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blog_Post");

            migrationBuilder.DropTable(
                name: "Blog_Tag");

            migrationBuilder.DropTable(
                name: "Core_Meta");

            migrationBuilder.DropTable(
                name: "Blog_Category");
        }
    }
}
