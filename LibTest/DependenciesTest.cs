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
            var query = Fluent.Default.WithNonQueryStatement("use Test")
                                      .WithNonQueryStatement("insert into TestTable (a, b) values ({0}, {1})")
                                          .WithDependency("select b*10 from TestTable where a = 5")
                                          .WithDependency("select b*10 from TestTable where a = 7");

            query.Execute();

            Tuple<int, int> theReturn = Fluent.Default.WithNonQueryStatement("use Test")
                                                      .WithReaderQueryStatement<Tuple<int, int>>("select a, b from TestTable where a not in (5, 7)", ReadFromTest)
                                                      .Execute()
                                                      .First();
            Assert.AreEqual(Tuple.Create(60, 80), theReturn);
        }

        [TestMethod]
        public void GivenAQueryWithANestedDependency_WhenExecutingTheQuery_TheDependencyChainMustBeMaintained()
        {
            var def = Fluent.Default;
            var query = Fluent.Default.WithNonQueryStatement("use Test")
                                      .WithScalarQueryStatement<int>("select 1 + {0}")
                                          .WithDependency<int>(def.WithScalarQueryStatement<Object>("select 2 + {0}")
                                                                      .WithDependency("select 3"));

            var theReturn = query.Execute();

            Assert.AreEqual(6, theReturn);
        }

        private Tuple<int, int> ReadFromTest(SqlDataReader reader)
        {
            return Tuple.Create(reader.GetInt32(0), reader.GetInt32(1));
        }
    }
}
