using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatApp.Migrations
{
    public partial class notifV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ToUserId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ToUserId",
                table: "Notifications",
                column: "ToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_ToUserId",
                table: "Notifications",
                column: "ToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_ToUserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ToUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ToUserId",
                table: "Notifications");
        }
    }
}
