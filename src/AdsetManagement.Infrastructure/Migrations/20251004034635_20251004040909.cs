using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdsetManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20251004040909 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imagens = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ano = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Km = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Cor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherOptions_ArCondicionado = table.Column<bool>(type: "bit", nullable: true),
                    OtherOptions_Alarme = table.Column<bool>(type: "bit", nullable: true),
                    OtherOptions_Airbag = table.Column<bool>(type: "bit", nullable: true),
                    OtherOptions_ABS = table.Column<bool>(type: "bit", nullable: true),
                    PacoteICarros = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PacoteWebMotors = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<int>(type: "int", maxLength: 100, nullable: true),
                    UpdateUserId = table.Column<int>(type: "int", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
