﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using PTV.Database.DataAccess.Migrations.Base;
using PTV.Database.DataAccess.Utils;

namespace PTV.Database.DataAccess.Migrations
{
    internal partial class ServiceMunicipalityRemove : IPartialMigration
    {
        private DataUtils dataUtils;

        public ServiceMunicipalityRemove()
        {
            dataUtils = new DataUtils();
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
			dataUtils.AddSqlScript(migrationBuilder, Path.Combine("SqlMigrations", "PTV_1_4_5", "Fixed", "6ServiceMunicipality.sql"));

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceCoverageType_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropTable(
                name: "ServiceCoverageTypeName",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceMunicipality",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ServiceCoverageType",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Ser_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned");

            migrationBuilder.DropColumn(
                name: "ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned");
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceCoverageType",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCoverageType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceMunicipality",
                schema: "public",
                columns: table => new
                {
                    ServiceVersionedId = table.Column<Guid>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceMunicipality", x => new { x.ServiceVersionedId, x.MunicipalityId });
                    table.ForeignKey(
                        name: "FK_ServiceMunicipality_Municipality_MunicipalityId",
                        column: x => x.MunicipalityId,
                        principalSchema: "public",
                        principalTable: "Municipality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceMunicipality_ServiceVersioned_ServiceVersionedId",
                        column: x => x.ServiceVersionedId,
                        principalSchema: "public",
                        principalTable: "ServiceVersioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCoverageTypeName",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LocalizationId = table.Column<Guid>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCoverageTypeName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCoverageTypeName_Language_LocalizationId",
                        column: x => x.LocalizationId,
                        principalSchema: "public",
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceCoverageTypeName_ServiceCoverageType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "public",
                        principalTable: "ServiceCoverageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SerVer_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "ServiceCoverageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerCovTyp_Id",
                schema: "public",
                table: "ServiceCoverageType",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerCovTypNam_Id",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SerCovTypNam_LocalizationId",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SerCovTypNam_TypeId",
                schema: "public",
                table: "ServiceCoverageTypeName",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SerMun_MunicipalityId",
                schema: "public",
                table: "ServiceMunicipality",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_SerMun_ServiceVersionedId",
                schema: "public",
                table: "ServiceMunicipality",
                column: "ServiceVersionedId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceVersioned_ServiceCoverageType_ServiceCoverageTypeId",
                schema: "public",
                table: "ServiceVersioned",
                column: "ServiceCoverageTypeId",
                principalSchema: "public",
                principalTable: "ServiceCoverageType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
