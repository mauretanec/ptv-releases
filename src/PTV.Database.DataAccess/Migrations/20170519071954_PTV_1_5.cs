﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.DataAccess.Migrations.Base;

namespace PTV.Database.DataAccess.Migrations
{
    public partial class PTV_1_5 : Migration
    {
        readonly List<IPartialMigration> migrations = new List<IPartialMigration>
        {
           new ValidityFixAndBackgroundDesc(),
           new ServiceServiceChannelChange(),
           new ServiceHoursOrderNumber(),
           new ServiceFundingType(),
           new ServiceVoucher(),
           new EmailAndPhoneOrder(),
           new OperationIdInfo(),
           new APP_DATA_TABLE(),
           new AreaIsValid(),
           new KeywordMaxLengthRule()
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrations.ForEach(m =>
            {
                m.Up(migrationBuilder);
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrations.ForEach(m =>
            {
                m.Down(migrationBuilder);
            });
        }
    }
}
