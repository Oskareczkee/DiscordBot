using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBotNumeroDos.Dal.Migrations.Migrations
{
    public partial class UpdateKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentItems_Items_ItemID",
                table: "EquipmentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileItems_Items_ItemID",
                table: "ProfileItems");

            migrationBuilder.AlterColumn<int>(
                name: "ItemID",
                table: "ProfileItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ItemID",
                table: "EquipmentItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentItems_Items_ItemID",
                table: "EquipmentItems",
                column: "ItemID",
                principalTable: "Items",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileItems_Items_ItemID",
                table: "ProfileItems",
                column: "ItemID",
                principalTable: "Items",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentItems_Items_ItemID",
                table: "EquipmentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileItems_Items_ItemID",
                table: "ProfileItems");

            migrationBuilder.AlterColumn<int>(
                name: "ItemID",
                table: "ProfileItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ItemID",
                table: "EquipmentItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentItems_Items_ItemID",
                table: "EquipmentItems",
                column: "ItemID",
                principalTable: "Items",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileItems_Items_ItemID",
                table: "ProfileItems",
                column: "ItemID",
                principalTable: "Items",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
