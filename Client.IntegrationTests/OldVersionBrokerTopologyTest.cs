using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestContainers.Core.Builders;
using TestContainers.Core.Containers;
using Zeebe.Client;
using Zeebe.Client.Api.Responses;

namespace Client.IntegrationTests
{
    [TestFixture]
    public class OldVersionBrokerTopologyTest
    {
        private const string Version = "0.22.2";
        private readonly ZeebeIntegrationTestHelper testHelper = ZeebeIntegrationTestHelper.ofVersion(Version);
        private IZeebeClient zeebeClient;

        [OneTimeSetUp]
        public async Task Setup()
        {
            zeebeClient = await testHelper.SetupIntegrationTest();
        }

        [OneTimeTearDown]
        public async Task Stop()
        {
            await testHelper.TearDownIntegrationTest();
        }

        [Test]
        public async Task RequestTopology()
        {
            // given

            // when
            var topology = await zeebeClient.TopologyRequest().Send();

            // then
            Console.WriteLine(topology);

            var gatewayVersion = topology.GatewayVersion;
            Assert.AreEqual("lower then 0.23", gatewayVersion);

            var topologyBrokers = topology.Brokers;
            Assert.AreEqual(1, topologyBrokers.Count);

            var topologyBroker = topologyBrokers[0];
            Assert.AreEqual(0, topologyBroker.NodeId);

            // assert host and port ?!
            // Assert.AreEqual(container.GetMappedPort(26500), topologyBroker.Port);

            var partitions = topologyBroker.Partitions;
            Assert.AreEqual(1, partitions.Count);

            var partitionInfo = partitions[0];
            Assert.AreEqual(PartitionBrokerRole.LEADER, partitionInfo.Role);
            Assert.AreEqual(true, partitionInfo.IsLeader);
            Assert.AreEqual(1, partitionInfo.PartitionId);
        }
    }
}