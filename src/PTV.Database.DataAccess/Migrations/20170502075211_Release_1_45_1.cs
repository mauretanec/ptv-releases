﻿/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class Release_1_45_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriorityFallback",
                schema: "public",
                table: "PublishingStatusType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UseOrg_OrganizationId",
                schema: "public",
                table: "UserOrganization",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrganization_Organization_OrganizationId",
                schema: "public",
                table: "UserOrganization",
                column: "OrganizationId",
                principalSchema: "public",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrganization_Organization_OrganizationId",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropIndex(
                name: "IX_UseOrg_OrganizationId",
                schema: "public",
                table: "UserOrganization");

            migrationBuilder.DropColumn(
                name: "PriorityFallback",
                schema: "public",
                table: "PublishingStatusType");
        }
    }
}
