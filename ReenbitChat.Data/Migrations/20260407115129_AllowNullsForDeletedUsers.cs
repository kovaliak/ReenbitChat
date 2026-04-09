using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReenbitChat.Data.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullsForDeletedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chat_rooms_AspNetUsers_CreatorId",
                table: "chat_rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_AspNetUsers_UserId",
                table: "messages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "messages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "chat_rooms",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_chat_rooms_AspNetUsers_CreatorId",
                table: "chat_rooms",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_AspNetUsers_UserId",
                table: "messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chat_rooms_AspNetUsers_CreatorId",
                table: "chat_rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_AspNetUsers_UserId",
                table: "messages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "chat_rooms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_chat_rooms_AspNetUsers_CreatorId",
                table: "chat_rooms",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_AspNetUsers_UserId",
                table: "messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
