﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentSql;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;

namespace LibTest
{
    [TestClass]
    public class IntegrationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Fluent.Default.WithNonQueryStatement("if exists(select * from sys.databases where name = 'Test') drop database Test").Execute();
            Fluent.Default.WithNonQueryStatement("create database Test");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Fluent.Default.WithNonQueryStatement("if exists(select * from sys.databases where name = 'Test') drop database Test").Execute();
        }

        [TestMethod]
        public void GivenAQueryToCreateADbWithASingleTable_WhenExecutingTheQuery_TheMasterDbWillContainTheTable()
        {
            var query = Fluent.Default.WithNonQueryStatement("create database Test")
                                      .WithNonQueryStatement("use Test")
                                      .WithNonQueryStatement("create table TestTable (a int, b int)");

            query.Execute();

            Assert.AreEqual<String>("TestTable", TestHelper.GetTable());
        }

        [TestMethod]
        public void GivenAQueryToGetTheTablesFromTheDatabase_WhenExecutingTheQueryAsScalar_TheValueIsReturned()
        {
            var query = Fluent.Default.WithNonQueryStatement("create database Test")
                                      .WithNonQueryStatement("use Test")
                                      .WithNonQueryStatement("create table TestTable (a int, b int)")
                                      .WithNonQueryStatement("insert into TestTable (a , b) values (5, 6)")
                                      .WithScalarQueryStatement<int>("select a from TestTable");

            var result = query.Execute();

            Assert.AreEqual<int>(5, result);
        }

        [TestMethod]
        public void GivenAQueryToGetTheTablesFromTheDatabase_WhenExecutingTheQueryAsScalar_TheStringValueIsReturned()
        {
            var query = Fluent.Default.WithNonQueryStatement("create database Test")
                                      .WithNonQueryStatement("use Test")
                                      .WithNonQueryStatement("create table TestTable (a varchar(10))")
                                      .WithNonQueryStatement("insert into TestTable (a) values ('hello')")
                                      .WithScalarQueryStatement<String>("select a from TestTable");

            var result = query.Execute();

            Assert.AreEqual<String>("hello", result);
        }

        [TestMethod]
        public void GivenAQueryToGetTheTablesFromTheDatabase_WhenExecutingTheQueryAsResultSet_TheValuesAreReturned()
        {
            var query = Fluent.Default.WithNonQueryStatement("create database Test")
                                      .WithNonQueryStatement("use Test")
                                      .WithNonQueryStatement("create table TestTable (a int, b varchar(10))")
                                      .WithNonQueryStatement("insert into TestTable (a , b) values (5, 'hello')")
                                      .WithNonQueryStatement("insert into TestTable (a , b) values (7, 'world')")
                                      .WithReaderQueryStatement("select a, b from TestTable", TheReader);

            var result = query.Execute().ToList();

            var expected = new Object[]{ Tuple.Create(5, "hello"), Tuple.Create(7, "world")};

            Assert.AreEqual(expected[0], result[0]);
            Assert.AreEqual(expected[1], result[1]);
        }

        public Tuple<int, String> TheReader(SqlDataReader reader)
        {
            return Tuple.Create(reader.GetInt32(0), reader.GetString(1));
        }
    }
}
