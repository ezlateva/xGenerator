using ExperienceGenerator.Data;
using Sitecore.ContentTesting.Data;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.ContentTesting.Model.Data.Items;
using Sitecore.ContentTesting;
using Sitecore.ContentTesting.Extensions;

namespace ExperienceGenerator.Client.Repositories
{
    public class TestRepository
    {
        private readonly IContentTestStore _testStore;

        public TestRepository()
        {
            _testStore = new SitecoreContentTestStore();
        }

        public List<TestItem> GetActiveTests()
        {
            List<TestItem> results = new List<TestItem>();
            var tests = _testStore.GetActiveTests();
            foreach (var test in tests)
            {
                Item item = Database.GetItem(test.Uri);
                if (item != null)
                {
                    TestDefinitionItem testDefinitionItem = TestDefinitionItem.Create(item);
                    if (testDefinitionItem != null)
                    {

                        Item hostItem = (test.HostItemUri != null) ? item.Database.GetItem(test.HostItemUri) : null;
                        if (hostItem != null)
                        {
                            ITestConfiguration testConfiguration = this._testStore.LoadTestForItem(hostItem, testDefinitionItem);
                            if (testConfiguration != null)
                            {

                                TestItem testItem = new TestItem()
                                {
                                    TestName = hostItem.DisplayName,
                                    Variations = testConfiguration.TestSet.GetExperienceCount(),
                                    TestId = testDefinitionItem.InnerItem.ID.ToString()
                                };

                                results.Add(testItem);
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
