using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentSql;
using System.Data.SqlClient;
using FunctionalLib;

namespace LibTest
{
    [TestClass]
    public class FunctionTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Fluent.Default.WithNonQueryStatement("if exists(select * from sys.databases where name = 'Test') drop database Test").Execute();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Fluent.Default.WithNonQueryStatement("if exists(select * from sys.databases where name = 'Test') drop database Test").Execute();
        }

        [TestMethod]
        public void GivenAQueryToCreateADbWithASingleTable_WhenExecutingTheQuery_TheMasterDbWillContainTheTable()
        {
            NonQueryAction function = CurryExtension.Curry<SqlConnection, String>(TestHelper.CreateDatabase, "Test").Invoke;
            var query = Fluent.Default.WithNonQueryAction(function)
                                      .WithNonQueryStatement("use Test")
                                      .WithNonQueryStatement("create table TestTable (a int, b int)");

            query.Execute();

            Assert.AreEqual<String>("TestTable", TestHelper.GetTable());
        }
    }
}
