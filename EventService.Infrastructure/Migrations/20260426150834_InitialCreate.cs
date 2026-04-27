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
                name: "LOCALISATIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", precision: 10, scale: 8, nullable: true),
                    LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", precision: 11, scale: 8, nullable: true),
                    ADRESSE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    VILLE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CODE_POSTAL = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOCALISATIONS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EVENEMENTS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TITRE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    START_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    END_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TYPE_EVENT = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CATEGORIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DISPONIBILITE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CAPACITE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PLACES_RESTANTES = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LIEN_PARTAGE = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ORGANISATEUR_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LOCALISATION_ID = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENEMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EVENEMENTS_LOCALISATIONS_LOCALISATION_ID",
                        column: x => x.LOCALISATION_ID,
                        principalTable: "LOCALISATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BILLET_TYPES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOM = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    PRIX = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    QUANTITE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VENDU = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    EVENEMENT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BILLET_TYPES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BILLET_TYPES_EVENEMENTS_EVENEMENT_ID",
                        column: x => x.EVENEMENT_ID,
                        principalTable: "EVENEMENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BILLETS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CODE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    STATUT = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValue: "Disponible"),
                    DATE_RESERVATION = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DATE_VALIDATION = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    VISITEUR_ID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    BILLET_TYPE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PAYMENT_TRANSACTION_ID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BILLETS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BILLETS_BILLET_TYPES_BILLET_TYPE_ID",
                        column: x => x.BILLET_TYPE_ID,
                        principalTable: "BILLET_TYPES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BILLET_TYPES_EVENEMENT_ID",
                table: "BILLET_TYPES",
                column: "EVENEMENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BILLETS_BILLET_TYPE_ID",
                table: "BILLETS",
                column: "BILLET_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BILLETS_CODE",
                table: "BILLETS",
                column: "CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EVENEMENTS_LOCALISATION_ID",
                table: "EVENEMENTS",
                column: "LOCALISATION_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BILLETS");

            migrationBuilder.DropTable(
                name: "BILLET_TYPES");

            migrationBuilder.DropTable(
                name: "EVENEMENTS");

            migrationBuilder.DropTable(
                name: "LOCALISATIONS");
        }
    }
}
