using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentSql;
using System.Data.SqlClient;

namespace LibTest
{
    [TestClass]
    public class TransactionTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Fluent.Default.WithNonQueryStatement("if exists(select * from sys.databases where name = 'Test') drop database Test")
                          .WithNonQueryStatement("create database Test")
                          .WithNonQueryStatement("use Test")
                          .WithNonQueryStatement("create table TestTable (a int, b int)")
                          .WithNonQueryStatement("insert into TestTable (a , b) values (5, 6)")
                          .Execute();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Fluent.Default.WithNonQueryStatement("if exists(select * from sys.databases where name = 'Test') drop database Test").Execute();
        }

        [TestMethod]
        public void GivenACompositeQuery_WhenAPartOfTheQueryFails_TheEntireQueryShouldBeRolledBack()
        {
            try
            {
                NonQueryAction act = ThrowException;
                Fluent.Default.WithNonQueryStatement("use Test")
                              .WithNonQueryStatement("insert into TestTable (a , b) values (7, 8)")
                              .WithNonQueryAction(act)
                              .ExecuteAsTransaction();
            }
            catch (Exception)
            {
                var x = Fluent.Default.WithNonQueryStatement("use Test")
                              .WithScalarQueryStatement<int>("select count (*) from TestTable")
                              .Execute();
                Assert.AreEqual(1, x);
            }
        }

        public static void ThrowException(SqlConnection conn, SqlTransaction trans, params Object[] notUsed)
        {
            throw new Exception("Kupow");
        }
    }
}
