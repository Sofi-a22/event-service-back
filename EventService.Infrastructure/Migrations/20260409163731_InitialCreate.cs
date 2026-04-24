using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Localisations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Adresse = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ville = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CodePostal = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localisations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nom = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Verifie = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Visiteurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nom = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visiteurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Evenements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Titre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TypeEvent = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Categorie = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Disponibilite = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Capacite = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PlacesRestantes = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LienPartage = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    OrganisateurId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LocalisationId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evenements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evenements_Localisations_LocalisationId",
                        column: x => x.LocalisationId,
                        principalTable: "Localisations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Evenements_Organisateurs_OrganisateurId",
                        column: x => x.OrganisateurId,
                        principalTable: "Organisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BilletTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nom = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Prix = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    Capacity = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Sold = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EvenementId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BilletTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BilletTypes_Evenements_EvenementId",
                        column: x => x.EvenementId,
                        principalTable: "Evenements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoris",
                columns: table => new
                {
                    EvenementId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VisiteurId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoris", x => new { x.EvenementId, x.VisiteurId });
                    table.ForeignKey(
                        name: "FK_Favoris_Evenements_EvenementId",
                        column: x => x.EvenementId,
                        principalTable: "Evenements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoris_Visiteurs_VisiteurId",
                        column: x => x.VisiteurId,
                        principalTable: "Visiteurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Billets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Statut = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    VisiteurId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    BilletTypeId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billets_BilletTypes_BilletTypeId",
                        column: x => x.BilletTypeId,
                        principalTable: "BilletTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Billets_Visiteurs_VisiteurId",
                        column: x => x.VisiteurId,
                        principalTable: "Visiteurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Billets_BilletTypeId",
                table: "Billets",
                column: "BilletTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Billets_VisiteurId",
                table: "Billets",
                column: "VisiteurId");

            migrationBuilder.CreateIndex(
                name: "IX_BilletTypes_EvenementId",
                table: "BilletTypes",
                column: "EvenementId");

            migrationBuilder.CreateIndex(
                name: "IX_Evenements_LocalisationId",
                table: "Evenements",
                column: "LocalisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Evenements_OrganisateurId",
                table: "Evenements",
                column: "OrganisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoris_VisiteurId",
                table: "Favoris",
                column: "VisiteurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Billets");

            migrationBuilder.DropTable(
                name: "Favoris");

            migrationBuilder.DropTable(
                name: "BilletTypes");

            migrationBuilder.DropTable(
                name: "Visiteurs");

            migrationBuilder.DropTable(
                name: "Evenements");

            migrationBuilder.DropTable(
                name: "Localisations");

            migrationBuilder.DropTable(
                name: "Organisateurs");
        }
    }
}
