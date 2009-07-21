using System;
using System.Collections.Generic;
using System.IO;
using Horn.Core.BuildEngines;
using Horn.Core.Dsl;
using Horn.Core.GetOperations;
using Horn.Core.PackageCommands;
using Horn.Core.PackageStructure;
using Horn.Core.SCM;
using Horn.Core.Spec.Doubles;
using Horn.Core.Spec.helpers;
using Horn.Core.Spec.Stubs;
using Horn.Core.Spec.Unit.GetSpecs;
using Horn.Framework.helpers;
using Horn.Spec.Framework.doubles;
using Rhino.Mocks;
using Xunit;

namespace Horn.Core.Spec.PrebuildSpecs
{
    public class When_the_requested_project_contains : GetSpecificationBase
    {
        private MockRepository mockRepository;

        private PackageBuilder packageBuilder;

        protected override void Before_each_spec()
        {
            mockRepository = new MockRepository();

            packageTree = TreeHelper.GetTempPackageTree().RetrievePackage(PackageTreeHelper.PackageWithPatch);

            get = MockRepository.GenerateStub<IGet>();

            get.Stub(x => x.From(new SVNSourceControl("url"))).Return(get);

            get.Stub(x => x.ExportTo(packageTree)).Return(packageTree);

            buildMetaData = TreeHelper.GetPackageTreeParts(new List<Dependency>());

            packageBuilder = new PackageBuilderWithOnlyPrebuildStub(get, new DiagnosticsProcessFactory(), new CommandArgsDouble("log4net", true), buildMetaData);
        }

        protected override void Because()
        {
            mockRepository.Playback();

            packageBuilder.Execute(packageTree);          
        }

        [Fact]
        public void Then_the_patch_files_are_xcopied()
        {
            var patchDirectory = Path.Combine(packageTree.RetrievePackage("log4net").CurrentDirectory.FullName, "patch");
            var childDirectory = Path.Combine(patchDirectory, "child");
            var testFile = Path.Combine(childDirectory, "patchfile.txt");

            Assert.True(File.Exists(testFile));
        }
    }
}