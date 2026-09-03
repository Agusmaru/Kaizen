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
                name: "AreasPersonales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Metas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreaPersonalId = table.Column<int>(type: "int", nullable: false),
                    PorQueEsImportante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SituacionActual = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResultadoEsperado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaObjetivo = table.Column<DateOnly>(type: "date", nullable: true),
                    MetricaProgreso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FrecuenciaRevision = table.Column<int>(type: "int", nullable: false),
                    FechaProximaRevision = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_PersonalAreas_PersonalAreaId",
                        column: x => x.AreaPersonalId,
                        principalTable: "AreasPersonales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccionesPlanificadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frecuencia = table.Column<int>(type: "int", nullable: false),
                    DiasSemana = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hora = table.Column<TimeOnly>(type: "time", nullable: true),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: true),
                    DificultadEstimada = table.Column<int>(type: "int", nullable: false),
                    CantidadObjetivo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnidadMetrica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionPlanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionPlanItems_Goals_GoalId",
                        column: x => x.MetaId,
                        principalTable: "Metas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesKaizen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetaId = table.Column<int>(type: "int", nullable: false),
                    DondeEstoy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueCambiar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Obstaculos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueFunciona = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MejoraMasPequena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvidenciaMejora = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DificultadPercibida = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KaizenAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KaizenAssessments_Goals_GoalId",
                        column: x => x.MetaId,
                        principalTable: "Metas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevisionesKaizen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetaId = table.Column<int>(type: "int", nullable: false),
                    InicioPeriodo = table.Column<DateOnly>(type: "date", nullable: false),
                    FinPeriodo = table.Column<DateOnly>(type: "date", nullable: false),
                    PorcentajeCumplimiento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    QueFunciono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueDificulto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FueDemasiadoDificil = table.Column<bool>(type: "bit", nullable: false),
                    AjustePequeno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SugerenciaAutomatica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KaizenReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KaizenReviews_Goals_GoalId",
                        column: x => x.MetaId,
                        principalTable: "Metas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccionesProgramadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccionPlanificadaId = table.Column<int>(type: "int", nullable: false),
                    FechaProgramada = table.Column<DateOnly>(type: "date", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledActions_ActionPlanItems_ActionPlanItemId",
                        column: x => x.AccionPlanificadaId,
                        principalTable: "AccionesPlanificadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AjustesPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RevisionKaizenId = table.Column<int>(type: "int", nullable: false),
                    AccionPlanificadaId = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanAdjustments_ActionPlanItems_ActionPlanItemId",
                        column: x => x.AccionPlanificadaId,
                        principalTable: "AccionesPlanificadas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlanAdjustments_KaizenReviews_KaizenReviewId",
                        column: x => x.RevisionKaizenId,
                        principalTable: "RevisionesKaizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccionProgramadaId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Nota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorReal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionLogs_ScheduledActions_ScheduledActionId",
                        column: x => x.AccionProgramadaId,
                        principalTable: "AccionesProgramadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLogs_ScheduledActionId",
                table: "RegistrosAccion",
                column: "AccionProgramadaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActionPlanItems_GoalId",
                table: "AccionesPlanificadas",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_PersonalAreaId",
                table: "Metas",
                column: "AreaPersonalId");

            migrationBuilder.CreateIndex(
                name: "IX_KaizenAssessments_GoalId",
                table: "EvaluacionesKaizen",
                column: "MetaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KaizenReviews_GoalId",
                table: "RevisionesKaizen",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAdjustments_ActionPlanItemId",
                table: "AjustesPlan",
                column: "AccionPlanificadaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAdjustments_KaizenReviewId",
                table: "AjustesPlan",
                column: "RevisionKaizenId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledActions_ActionPlanItemId_ScheduledDate",
                table: "AccionesProgramadas",
                columns: new[] { "AccionPlanificadaId", "FechaProgramada" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosAccion");

            migrationBuilder.DropTable(
                name: "EvaluacionesKaizen");

            migrationBuilder.DropTable(
                name: "AjustesPlan");

            migrationBuilder.DropTable(
                name: "AccionesProgramadas");

            migrationBuilder.DropTable(
                name: "RevisionesKaizen");

            migrationBuilder.DropTable(
                name: "AccionesPlanificadas");

            migrationBuilder.DropTable(
                name: "Metas");

            migrationBuilder.DropTable(
                name: "AreasPersonales");
        }
    }
}
