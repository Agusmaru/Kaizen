using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kaizen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActionVersioningAndReviewHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_KaizenReviews_GoalId",
                table: "KaizenReviews");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveDate",
                table: "PlanAdjustments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "NewActionPlanItemId",
                table: "PlanAdjustments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                table: "PlanAdjustments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousValues",
                table: "PlanAdjustments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "PlanAdjustments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ResumeDate",
                table: "PlanAdjustments",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "KaizenReviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalStatusAtEnd",
                table: "KaizenReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalStatusAtStart",
                table: "KaizenReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Learned",
                table: "KaizenReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NextPeriodChange",
                table: "KaizenReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "NextReviewDate",
                table: "KaizenReviews",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivatedAt",
                table: "Goals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Goals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ActionSeriesId",
                table: "ActionPlanItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ChangeReason",
                table: "ActionPlanItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveFrom",
                table: "ActionPlanItems",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "EffectiveTo",
                table: "ActionPlanItems",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginReviewId",
                table: "ActionPlanItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviousVersionId",
                table: "ActionPlanItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "ActionPlanItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Backfill conservador: cada acción existente se convierte en la versión 1
            // de su propia serie y mantiene su fecha inicial como comienzo de vigencia.
            migrationBuilder.Sql("""
                UPDATE ActionPlanItems
                SET ActionSeriesId = NEWID(), Version = 1, EffectiveFrom = StartDate;

                UPDATE Goals
                SET CreatedAt = CAST(StartDate AS datetime2),
                    ActivatedAt = CASE WHEN Status = 1 THEN CAST(StartDate AS datetime2) ELSE NULL END;

                UPDATE r
                SET NextReviewDate = DATEADD(day, 7, r.PeriodEnd),
                    GoalStatusAtStart = g.Status,
                    GoalStatusAtEnd = g.Status,
                    Learned = CASE WHEN r.Learned = '' THEN 'Sin registro histórico' ELSE r.Learned END,
                    NextPeriodChange = CASE WHEN r.NextPeriodChange = '' THEN r.SmallAdjustment ELSE r.NextPeriodChange END
                FROM KaizenReviews r
                INNER JOIN Goals g ON g.Id = r.GoalId;

                UPDATE p
                SET EffectiveDate = DATEADD(day, 1, r.PeriodEnd)
                FROM PlanAdjustments p
                INNER JOIN KaizenReviews r ON r.Id = p.KaizenReviewId;
                """);

            migrationBuilder.CreateTable(
                name: "GoalHistoryEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    ActionPlanItemId = table.Column<int>(type: "int", nullable: true),
                    KaizenReviewId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalHistoryEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoalHistoryEvents_ActionPlanItems_ActionPlanItemId",
                        column: x => x.ActionPlanItemId,
                        principalTable: "ActionPlanItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoalHistoryEvents_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalHistoryEvents_KaizenReviews_KaizenReviewId",
                        column: x => x.KaizenReviewId,
                        principalTable: "KaizenReviews",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledActions_ScheduledDate",
                table: "ScheduledActions",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAdjustments_NewActionPlanItemId",
                table: "PlanAdjustments",
                column: "NewActionPlanItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KaizenReviews_GoalId_PeriodStart_PeriodEnd",
                table: "KaizenReviews",
                columns: new[] { "GoalId", "PeriodStart", "PeriodEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlanItems_ActionSeriesId_EffectiveFrom",
                table: "ActionPlanItems",
                columns: new[] { "ActionSeriesId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlanItems_OriginReviewId",
                table: "ActionPlanItems",
                column: "OriginReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlanItems_PreviousVersionId",
                table: "ActionPlanItems",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalHistoryEvents_ActionPlanItemId",
                table: "GoalHistoryEvents",
                column: "ActionPlanItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalHistoryEvents_GoalId_OccurredAt",
                table: "GoalHistoryEvents",
                columns: new[] { "GoalId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GoalHistoryEvents_KaizenReviewId",
                table: "GoalHistoryEvents",
                column: "KaizenReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionPlanItems_ActionPlanItems_PreviousVersionId",
                table: "ActionPlanItems",
                column: "PreviousVersionId",
                principalTable: "ActionPlanItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionPlanItems_KaizenReviews_OriginReviewId",
                table: "ActionPlanItems",
                column: "OriginReviewId",
                principalTable: "KaizenReviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanAdjustments_ActionPlanItems_NewActionPlanItemId",
                table: "PlanAdjustments",
                column: "NewActionPlanItemId",
                principalTable: "ActionPlanItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionPlanItems_ActionPlanItems_PreviousVersionId",
                table: "ActionPlanItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ActionPlanItems_KaizenReviews_OriginReviewId",
                table: "ActionPlanItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanAdjustments_ActionPlanItems_NewActionPlanItemId",
                table: "PlanAdjustments");

            migrationBuilder.DropTable(
                name: "GoalHistoryEvents");

            migrationBuilder.DropIndex(
                name: "IX_ScheduledActions_ScheduledDate",
                table: "ScheduledActions");

            migrationBuilder.DropIndex(
                name: "IX_PlanAdjustments_NewActionPlanItemId",
                table: "PlanAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_KaizenReviews_GoalId_PeriodStart_PeriodEnd",
                table: "KaizenReviews");

            migrationBuilder.DropIndex(
                name: "IX_ActionPlanItems_ActionSeriesId_EffectiveFrom",
                table: "ActionPlanItems");

            migrationBuilder.DropIndex(
                name: "IX_ActionPlanItems_OriginReviewId",
                table: "ActionPlanItems");

            migrationBuilder.DropIndex(
                name: "IX_ActionPlanItems_PreviousVersionId",
                table: "ActionPlanItems");

            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "PlanAdjustments");

            migrationBuilder.DropColumn(
                name: "NewActionPlanItemId",
                table: "PlanAdjustments");

            migrationBuilder.DropColumn(
                name: "NewValues",
                table: "PlanAdjustments");

            migrationBuilder.DropColumn(
                name: "PreviousValues",
                table: "PlanAdjustments");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "PlanAdjustments");

            migrationBuilder.DropColumn(
                name: "ResumeDate",
                table: "PlanAdjustments");

            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "KaizenReviews");

            migrationBuilder.DropColumn(
                name: "GoalStatusAtEnd",
                table: "KaizenReviews");

            migrationBuilder.DropColumn(
                name: "GoalStatusAtStart",
                table: "KaizenReviews");

            migrationBuilder.DropColumn(
                name: "Learned",
                table: "KaizenReviews");

            migrationBuilder.DropColumn(
                name: "NextPeriodChange",
                table: "KaizenReviews");

            migrationBuilder.DropColumn(
                name: "NextReviewDate",
                table: "KaizenReviews");

            migrationBuilder.DropColumn(
                name: "ActivatedAt",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "ActionSeriesId",
                table: "ActionPlanItems");

            migrationBuilder.DropColumn(
                name: "ChangeReason",
                table: "ActionPlanItems");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "ActionPlanItems");

            migrationBuilder.DropColumn(
                name: "EffectiveTo",
                table: "ActionPlanItems");

            migrationBuilder.DropColumn(
                name: "OriginReviewId",
                table: "ActionPlanItems");

            migrationBuilder.DropColumn(
                name: "PreviousVersionId",
                table: "ActionPlanItems");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ActionPlanItems");

            migrationBuilder.CreateIndex(
                name: "IX_KaizenReviews_GoalId",
                table: "KaizenReviews",
                column: "GoalId");
        }
    }
}
