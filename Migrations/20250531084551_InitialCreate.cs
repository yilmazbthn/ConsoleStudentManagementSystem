using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomInstructor_Users_InstructorIdsId",
                table: "ClassroomInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomStudent_Users_StudentIdsId",
                table: "ClassroomStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_Homework_Users_StudentId",
                table: "Homework");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Homework",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "TeachingLessonIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClassroomIds",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "InstructorIds",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "Homework",
                newName: "Homeworks");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Lessons",
                newName: "Schedule");

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

            migrationBuilder.RenameIndex(
                name: "IX_Homework_StudentId",
                table: "Homeworks",
                newName: "IX_Homeworks_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Homeworks",
                table: "Homeworks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ClassroomInstructors",
                columns: table => new
                {
                    ClassroomId = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomInstructors", x => new { x.ClassroomId, x.InstructorId });
                    table.ForeignKey(
                        name: "FK_ClassroomInstructors_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassroomInstructors_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassroomLesson",
                columns: table => new
                {
                    ClassroomsId = table.Column<int>(type: "int", nullable: false),
                    LessonsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassroomLesson", x => new { x.ClassroomsId, x.LessonsId });
                    table.ForeignKey(
                        name: "FK_ClassroomLesson_Classrooms_ClassroomsId",
                        column: x => x.ClassroomsId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassroomLesson_Lessons_LessonsId",
                        column: x => x.LessonsId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorLesson",
                columns: table => new
                {
                    InstructorsId = table.Column<int>(type: "int", nullable: false),
                    LessonsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorLesson", x => new { x.InstructorsId, x.LessonsId });
                    table.ForeignKey(
                        name: "FK_InstructorLesson_Lessons_LessonsId",
                        column: x => x.LessonsId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorLesson_Users_InstructorsId",
                        column: x => x.InstructorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentHomeworkGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    HomeworkId = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentHomeworkGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentHomeworkGrades_Homeworks_HomeworkId",
                        column: x => x.HomeworkId,
                        principalTable: "Homeworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentHomeworkGrades_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_LessonId",
                table: "Homeworks",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomInstructors_InstructorId",
                table: "ClassroomInstructors",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassroomLesson_LessonsId",
                table: "ClassroomLesson",
                column: "LessonsId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorLesson_LessonsId",
                table: "InstructorLesson",
                column: "LessonsId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentHomeworkGrades_HomeworkId",
                table: "StudentHomeworkGrades",
                column: "HomeworkId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentHomeworkGrades_StudentId",
                table: "StudentHomeworkGrades",
                column: "StudentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_Lessons_LessonId",
                table: "Homeworks",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_Users_StudentId",
                table: "Homeworks",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomInstructor_Users_InstructorsId",
                table: "ClassroomInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassroomStudent_Users_StudentsId",
                table: "ClassroomStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_Lessons_LessonId",
                table: "Homeworks");

            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_Users_StudentId",
                table: "Homeworks");

            migrationBuilder.DropTable(
                name: "ClassroomInstructors");

            migrationBuilder.DropTable(
                name: "ClassroomLesson");

            migrationBuilder.DropTable(
                name: "InstructorLesson");

            migrationBuilder.DropTable(
                name: "StudentHomeworkGrades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Homeworks",
                table: "Homeworks");

            migrationBuilder.DropIndex(
                name: "IX_Homeworks_LessonId",
                table: "Homeworks");

            migrationBuilder.RenameTable(
                name: "Homeworks",
                newName: "Homework");

            migrationBuilder.RenameColumn(
                name: "Schedule",
                table: "Lessons",
                newName: "Time");

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

            migrationBuilder.RenameIndex(
                name: "IX_Homeworks_StudentId",
                table: "Homework",
                newName: "IX_Homework_StudentId");

            migrationBuilder.AddColumn<string>(
                name: "TeachingLessonIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassroomIds",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstructorIds",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Homework",
                table: "Homework",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Homework_Users_StudentId",
                table: "Homework",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
