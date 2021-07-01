﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BoDi;
using Moq;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSource;
using SpecFlow.ExternalData.SpecFlowPlugin.Loaders;
using Xunit;

namespace SpecFlow.ExternalData.SpecFlowPlugin.UnitTests
{
    public class DataSourceLoaderFactoryTests
    {
        private readonly IObjectContainer _container = new ObjectContainer();

        private DataSourceLoaderFactory CreateSut()
        {
            return _container.Resolve<DataSourceLoaderFactory>();
        }

        class TestLoader : FileBasedLoader
        {
            public TestLoader(params string[] acceptedExtensions) : base("TEST", acceptedExtensions)
            {
            }

            protected override DataValue LoadDataSourceFromFilePath(string filePath, string sourceFilePath) => throw new NotImplementedException();
        }

        private IDataSourceLoader CreateLoader(params string[] extensions)
        {
            extensions ??= new[] { ".xxx" };
            var loader = new TestLoader(extensions);
            return loader;
            //var mock = new Mock<IDataSourceLoader>();
            //mock.Setup(l => l.AcceptsSourceFilePath(It.IsAny<string>()))
            //    .Returns(new Func<string, bool>(path => extensions.Contains(Path.GetExtension(path)))); 
            //return mock.Object;
        }

        [Fact]
        public void Should_be_able_to_create_a_registered_loader_from_format()
        {
            var loader1 = CreateLoader();
            var loader2 = CreateLoader();
            _container.RegisterInstanceAs(loader1, "L1");
            _container.RegisterInstanceAs(loader2, "L2");
            var sut = CreateSut();
            var result = sut.CreateLoader("L2", "");

            Assert.Same(loader2, result);
        }

        [Fact]
        public void Should_be_able_to_create_a_registered_loader_from_format_with_different_casing()
        {
            var loader1 = CreateLoader();
            var loader2 = CreateLoader();
            _container.RegisterInstanceAs(loader1, "L1");
            _container.RegisterInstanceAs(loader2, "L2");
            var sut = CreateSut();
            var result = sut.CreateLoader("l1", "");

            Assert.Same(loader1, result);
        }

        [Fact]
        public void Should_handle_invalid_format()
        {
            var loader1 = CreateLoader();
            var loader2 = CreateLoader();
            _container.RegisterInstanceAs(loader1, "L1");
            _container.RegisterInstanceAs(loader2, "L2");
            var sut = CreateSut();
            
            Assert.Throws<ExternalDataPluginException>(() => 
                sut.CreateLoader("NO-SUCH-FORMAT", ""));
        }

        [Fact]
        public void Should_be_able_to_create_a_registered_loader_from_source_path_extension()
        {
            var loader1 = CreateLoader(".xlsx");
            var loader2 = CreateLoader(".csv");
            _container.RegisterInstanceAs(loader1, "L1");
            _container.RegisterInstanceAs(loader2, "L2");
            var sut = CreateSut();
            var result = sut.CreateLoader(null, "products.csv");

            Assert.Same(loader2, result);
        }

        [Fact]
        public void Should_be_able_to_create_a_registered_loader_from_source_path_extension_different_casing()
        {
            var loader1 = CreateLoader(".xlsx");
            var loader2 = CreateLoader(".csv");
            _container.RegisterInstanceAs(loader1, "L1");
            _container.RegisterInstanceAs(loader2, "L2");
            var sut = CreateSut();
            var result = sut.CreateLoader(null, "products.CSV");

            Assert.Same(loader2, result);
        }

        [Fact] public void Should_handle_when_no_loader_accepts_source_path()
        {
            var loader1 = CreateLoader(".xlsx");
            var loader2 = CreateLoader(".csv");
            _container.RegisterInstanceAs(loader1, "L1");
            _container.RegisterInstanceAs(loader2, "L2");
            var sut = CreateSut();

            Assert.Throws<ExternalDataPluginException>(() =>
                sut.CreateLoader(null, "source.abc"));
        }
    }

}
