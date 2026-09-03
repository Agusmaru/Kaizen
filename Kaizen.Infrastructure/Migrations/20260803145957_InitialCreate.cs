using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaizen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonalAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalAreaId = table.Column<int>(type: "int", nullable: false),
                    WhyImportant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentSituation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedOutcome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ProgressMetric = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewFrequency = table.Column<int>(type: "int", nullable: false),
                    NextReviewDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_PersonalAreas_PersonalAreaId",
                        column: x => x.PersonalAreaId,
                        principalTable: "PersonalAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionPlanItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    Weekdays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<TimeOnly>(type: "time", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EstimatedDifficulty = table.Column<int>(type: "int", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MetricUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionPlanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionPlanItems_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KaizenAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    WhereAmI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatToChange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Obstacles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatWorks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmallestImprovement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImprovementEvidence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerceivedDifficulty = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KaizenAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KaizenAssessments_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KaizenReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    CompletionPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    WhatWorked = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhatHindered = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WasTooDifficult = table.Column<bool>(type: "bit", nullable: false),
                    SmallAdjustment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutomaticSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KaizenReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KaizenReviews_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionPlanItemId = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledActions_ActionPlanItems_ActionPlanItemId",
                        column: x => x.ActionPlanItemId,
                        principalTable: "ActionPlanItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KaizenReviewId = table.Column<int>(type: "int", nullable: false),
                    ActionPlanItemId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanAdjustments_ActionPlanItems_ActionPlanItemId",
                        column: x => x.ActionPlanItemId,
                        principalTable: "ActionPlanItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlanAdjustments_KaizenReviews_KaizenReviewId",
                        column: x => x.KaizenReviewId,
                        principalTable: "KaizenReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduledActionId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionLogs_ScheduledActions_ScheduledActionId",
                        column: x => x.ScheduledActionId,
                        principalTable: "ScheduledActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ScheduledActionId",
                table: "ActionLogs",
                column: "ScheduledActionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlanItems_GoalId",
                table: "ActionPlanItems",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_PersonalAreaId",
                table: "Goals",
                column: "PersonalAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_KaizenAssessments_GoalId",
                table: "KaizenAssessments",
                column: "GoalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KaizenReviews_GoalId",
                table: "KaizenReviews",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAdjustments_ActionPlanItemId",
                table: "PlanAdjustments",
                column: "ActionPlanItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAdjustments_KaizenReviewId",
                table: "PlanAdjustments",
                column: "KaizenReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledActions_ActionPlanItemId_ScheduledDate",
                table: "ScheduledActions",
                columns: new[] { "ActionPlanItemId", "ScheduledDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLogs");

            migrationBuilder.DropTable(
                name: "KaizenAssessments");

            migrationBuilder.DropTable(
                name: "PlanAdjustments");

            migrationBuilder.DropTable(
                name: "ScheduledActions");

            migrationBuilder.DropTable(
                name: "KaizenReviews");

            migrationBuilder.DropTable(
                name: "ActionPlanItems");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "PersonalAreas");
        }
    }
}

