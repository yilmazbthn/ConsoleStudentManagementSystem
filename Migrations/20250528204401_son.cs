using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class son : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomInstructor_Users_InstructorsId",
                table: "ClassroomInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomStudent_Users_StudentsId",
                table: "ClassroomStudent");

            migrationBuilder.RenameColumn(
                name: "StudentsId",
                table: "ClassroomStudent",
                newName: "StudentIdsId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassroomStudent_StudentsId",
                table: "ClassroomStudent",
                newName: "IX_ClassroomStudent_StudentIdsId");

            migrationBuilder.RenameColumn(
                name: "InstructorsId",
                table: "ClassroomInstructor",
                newName: "InstructorIdsId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassroomInstructor_InstructorsId",
                table: "ClassroomInstructor",
                newName: "IX_ClassroomInstructor_InstructorIdsId");

            migrationBuilder.AddColumn<string>(
                name: "TeachingLessonIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassroomIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassroomInstructor_Users_InstructorIdsId",
                table: "ClassroomInstructor",
                column: "InstructorIdsId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassroomStudent_Users_StudentIdsId",
                table: "ClassroomStudent",
                column: "StudentIdsId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomInstructor_Users_InstructorIdsId",
                table: "ClassroomInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomStudent_Users_StudentIdsId",
                table: "ClassroomStudent");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropColumn(
                name: "TeachingLessonIds",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "StudentIdsId",
                table: "ClassroomStudent",
                newName: "StudentsId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassroomStudent_StudentIdsId",
                table: "ClassroomStudent",
                newName: "IX_ClassroomStudent_StudentsId");

            migrationBuilder.RenameColumn(
                name: "InstructorIdsId",
                table: "ClassroomInstructor",
                newName: "InstructorsId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassroomInstructor_InstructorIdsId",
                table: "ClassroomInstructor",
                newName: "IX_ClassroomInstructor_InstructorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassroomInstructor_Users_InstructorsId",
                table: "ClassroomInstructor",
                column: "InstructorsId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassroomStudent_Users_StudentsId",
                table: "ClassroomStudent",
                column: "StudentsId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
