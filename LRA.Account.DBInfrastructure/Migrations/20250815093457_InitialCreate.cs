using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LRA.Account.DBInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeycloakId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BlockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW()"),
                    IsTwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneTinePasswords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneTinePasswords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CredentialsDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsTemporary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW()"),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialsDetails_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kycs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IdentityDocumentPhoto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IdentityDocumentSelfie = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MedicalCertificatePhoto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminReviewId = table.Column<Guid>(type: "uuid", nullable: true),
                    RejectReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AccountEntityId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kycs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kycs_Accounts_AccountEntityId",
                        column: x => x.AccountEntityId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Kycs_Kycs_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Kycs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountRoles",
                columns: table => new
                {
                    AccountsId = table.Column<Guid>(type: "uuid", nullable: false),
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRoles", x => new { x.AccountsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_AccountRoles_Accounts_AccountsId",
                        column: x => x.AccountsId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountRoles_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "CreatedAtUtc", "Email", "FirstName", "KeycloakId", "LastName", "Phone", "UpdatedAtUtc" },
                values: new object[] { new Guid("cb67ce07-ad2c-4d16-9238-31c10b60306a"), new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc), "superadmin@mail.com", null, "f0b5af18-5352-4716-9ef4-c6654e5e7aeb", null, null, new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAtUtc", "Name", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("410434b8-9c7d-47be-a591-2bc1e6005d24"), new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc), "superAdmin", new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("766745d4-137e-42d5-b8b5-73d039a91720"), new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc), "admin", new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("85b823f0-4b6a-45fe-a44b-efd0df385ace"), new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc), "paramedic", new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d6a978a8-aa11-471c-be82-71ef221afdf0"), new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc), "client", new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AccountRoles",
                columns: new[] { "AccountsId", "RolesId" },
                values: new object[] { new Guid("cb67ce07-ad2c-4d16-9238-31c10b60306a"), new Guid("410434b8-9c7d-47be-a591-2bc1e6005d24") });

            migrationBuilder.InsertData(
                table: "CredentialsDetails",
                columns: new[] { "Id", "AccountId", "CreatedAtUtc", "UpdatedAtUtc" },
                values: new object[] { new Guid("d4f7b2e2-7b8e-4b7b-8f1a-6e7c9d8e7f9a"), new Guid("cb67ce07-ad2c-4d16-9238-31c10b60306a"), new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc), new DateTime(2023, 12, 25, 15, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRoles_RolesId",
                table: "AccountRoles",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationTokens_Token_UserEmail",
                table: "ConfirmationTokens",
                columns: new[] { "Token", "UserEmail" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CredentialsDetails_AccountId",
                table: "CredentialsDetails",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kycs_AccountEntityId",
                table: "Kycs",
                column: "AccountEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Kycs_AccountId",
                table: "Kycs",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OneTinePasswords_Password_UserEmail",
                table: "OneTinePasswords",
                columns: new[] { "Password", "UserEmail" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRoles");

            migrationBuilder.DropTable(
                name: "ConfirmationTokens");

            migrationBuilder.DropTable(
                name: "CredentialsDetails");

            migrationBuilder.DropTable(
                name: "Kycs");

            migrationBuilder.DropTable(
                name: "OneTinePasswords");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
