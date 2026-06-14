using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpMngtAPI.Migrations
{
    /// <inheritdoc />
    public partial class FkInJobTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_jobPositionTbls_LocationId",
                table: "jobPositionTbls",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_jobPositionTbls_locationTbls_LocationId",
                table: "jobPositionTbls",
                column: "LocationId",
                principalTable: "locationTbls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jobPositionTbls_locationTbls_LocationId",
                table: "jobPositionTbls");

            migrationBuilder.DropIndex(
                name: "IX_jobPositionTbls_LocationId",
                table: "jobPositionTbls");
        }
    }
}
