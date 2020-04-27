using Microsoft.EntityFrameworkCore.Migrations;

namespace KnowledgeSpace.BackendServer.Data.Migrations
{
    public partial class ReplyIdForComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyId",
                table: "Comments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyId",
                table: "Comments");
        }
    }
}
