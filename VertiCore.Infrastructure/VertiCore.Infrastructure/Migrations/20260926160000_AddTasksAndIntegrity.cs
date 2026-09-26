using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VertiCore.Infrastructure.Data;

#nullable disable

namespace VertiCore.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260926160000_AddTasksAndIntegrity")]
    public partial class AddTasksAndIntegrity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM ""Users""
                        GROUP BY LOWER(BTRIM(""Email""))
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'Duplicate user emails exist; resolve them before applying the unique-email migration.';
                    END IF;
                END $$;

                UPDATE ""Users"" SET ""Email"" = LOWER(BTRIM(""Email""));
                UPDATE ""Users"" SET ""UpdatedAt"" = ""CreatedAt"" WHERE ""UpdatedAt"" = TIMESTAMPTZ '0001-01-01 00:00:00+00';
                UPDATE ""Tenants"" SET ""UpdatedAt"" = ""CreatedAt"" WHERE ""UpdatedAt"" = TIMESTAMPTZ '0001-01-01 00:00:00+00';
                UPDATE ""Clients"" SET ""UpdatedAt"" = ""CreatedAt"" WHERE ""UpdatedAt"" = TIMESTAMPTZ '0001-01-01 00:00:00+00';
                UPDATE ""Invoices"" SET ""UpdatedAt"" = ""CreatedAt"" WHERE ""UpdatedAt"" = TIMESTAMPTZ '0001-01-01 00:00:00+00';
                UPDATE ""InvoiceItems"" SET ""UpdatedAt"" = ""CreatedAt"" WHERE ""UpdatedAt"" = TIMESTAMPTZ '0001-01-01 00:00:00+00';
                UPDATE ""UserInvitations"" SET ""UpdatedAt"" = ""CreatedAt"" WHERE ""UpdatedAt"" = TIMESTAMPTZ '0001-01-01 00:00:00+00';
                UPDATE ""AuditLogs"" SET ""UpdatedAt"" = ""CreatedAt"" WHERE ""UpdatedAt"" = TIMESTAMPTZ '0001-01-01 00:00:00+00';
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateTable(
                name: "WorkTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTasks", task => task.Id);
                    table.ForeignKey(
                        name: "FK_WorkTasks_Tenants_TenantId",
                        column: task => task.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkTasks_Users_AssignedUserId",
                        column: task => task.AssignedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_AssignedUserId",
                table: "WorkTasks",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_TenantId",
                table: "WorkTasks",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "WorkTasks");
            migrationBuilder.DropIndex(name: "IX_Users_Email", table: "Users");
        }
    }
}