using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchIncidences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsLocked",
                table: "Matches",
                newName: "RankingApplied");

            migrationBuilder.AddColumn<string>(
                name: "FlagBase64",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MatchIncidences",
                columns: table => new
                {
                    IncidenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: false),
                    IncidenceType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchIncidences", x => x.IncidenceId);
                    table.ForeignKey(
                        name: "FK_MatchIncidences_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchIncidences_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchIncidences_MatchId",
                table: "MatchIncidences",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchIncidences_TeamId",
                table: "MatchIncidences",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchIncidences");

            migrationBuilder.DropColumn(
                name: "FlagBase64",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "RankingApplied",
                table: "Matches",
                newName: "IsLocked");
        }
    }
}
