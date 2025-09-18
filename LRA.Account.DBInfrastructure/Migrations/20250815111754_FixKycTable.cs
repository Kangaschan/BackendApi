using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LRA.Account.DBInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixKycTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kycs_Kycs_AccountId",
                table: "Kycs");

            migrationBuilder.AddForeignKey(
                name: "FK_Kycs_Accounts_AccountId",
                table: "Kycs",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kycs_Accounts_AccountId",
                table: "Kycs");

            migrationBuilder.AddForeignKey(
                name: "FK_Kycs_Kycs_AccountId",
                table: "Kycs",
                column: "AccountId",
                principalTable: "Kycs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
