using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentSql;
using System.Linq;
using System.Data.SqlClient;

namespace LibTest
{
    [TestClass]
    public class DependenciesTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Fluent.Default.WithNonQueryStatement("if exists(select * from sys.databases where name = 'Test') drop database Test")
                          .WithNonQueryStatement("create database Test")
                          .WithNonQueryStatement("use Test")
                          .WithNonQueryStatement("create table TestTable (a int, b int)")
                          .WithNonQueryStatement("insert into TestTable (a , b) values (5, 6)")
                          .WithNonQueryStatement("insert into TestTable (a , b) values (7, 8)")
                          .Execute();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Fluent.Default.WithNonQueryStatement("if exists(select * from sys.databases where name = 'Test') drop database Test").Execute();
        }

        [TestMethod]
        public void GivenAQueryWithDependencies_WhenExecutingTheQuery_TheResultOfTheSubQueriesMustBeFedToTheMainQuery()
        {
            /*var query = Fluent.Default.WithNonQueryStatement("insert into TestTable (a, b) values ({0}, {1})")
                                          .WithDependency("select a from TestTable where a = 5")
                                          .WithDependency("select a from TestTable where a = 7");

            query.Execute();*/

            Tuple<int, int> theReturn = Fluent.Default.WithReaderQueryStatement<Tuple<int, int>>("select a, b from TestTable", ReadFromTest)
                                                      .Execute()
                                                      .First();
            Assert.AreEqual(Tuple.Create(0, 0), theReturn);
        }

        private Tuple<int, int> ReadFromTest(SqlDataReader reader)
        {
            return Tuple.Create(reader.GetInt32(0), reader.GetInt32(1));
        }
    }
}
