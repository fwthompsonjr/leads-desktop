﻿using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System;
using System.Linq;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.util
{
    public class ActionBexarContainerTests
    {
        [Fact]
        public void InstanceContainsContainer()
        {
            var error = Record.Exception(() =>
            {
                var item = ActionBexarContainer.GetContainer;
                Assert.NotNull(item);
            });
            Assert.Null(error);
        }

        [Fact]
        public void InstanceGetFetchCollection()
        {
            var error = Record.Exception(() =>
            {
                var item = ActionBexarContainer.GetContainer;
                var children = item.GetAllInstances<ICountySearchAction>();
                Assert.NotNull(children);
                Assert.Equal(9, children.Count());
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(typeof(IHttpService))]
        [InlineData(typeof(ICountyCodeService))]
        [InlineData(typeof(ICountyCodeReader))]
        public void ServiceCanGetTypedInstance(Type type)
        {
            var error = Record.Exception(() =>
            {
                var item = ActionBexarContainer.GetContainer;
                var actual = item.GetInstance(type);
                Assert.NotNull(actual);
            });
            Assert.Null(error);
        }
    }
}