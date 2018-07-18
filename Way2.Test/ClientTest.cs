using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Way2;
using Way2.Adapter;

namespace Way2.Test
{
    [TestClass]
    public class ClientTest
    {
        Mock<ISocket> socket;
        Client cliente;

        [TestInitialize]
        public void setUp()
        {
            socket = new Mock<ISocket>();
            cliente = new Client(socket.Object, "", 20003, 0, 10000);

        }

        [TestMethod]
        public void test_can_read_serial_number()
        {
            //Arrange
            socket.Setup(x => x.Receive(It.IsAny<byte[]>())).Returns(new byte[] { 0x7D, 0x08, 0x81, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x00, 0xC9 });

            //Act
            string serialNumber = cliente.GetNumeroSerie();

            //Assert
            Assert.AreEqual("ABCDEFG", serialNumber);
        }

        [TestMethod]
        public void test_can_read_register_status()
        {
            //Arrange
            socket.Setup(x => x.Receive(It.IsAny<byte[]>())).Returns(new byte[] { 0x7D, 0x04, 0x82, 0x01, 0x2C, 0x02, 0x58, 0xF1 });

            //Act
            int[] indices = cliente.GetRegistroStatus();

            //Assert
            Assert.AreEqual(300, indices[0]);
            Assert.AreEqual(600, indices[1]);
        }

        [TestMethod]
        public void test_can_read_datetime()
        {
            //Arrange
            socket.Setup(x => x.Receive(It.IsAny<byte[]>())).Returns(new byte[] { 0x7D, 0x05, 0x84, 0x7D, 0xE1, 0xBC, 0x59, 0x2B, 0xD3 });

            //Act
            var date = cliente.GetDataHora();

            //Assert
            Assert.AreEqual(new DateTime(2014, 01, 23, 17, 25, 10), date);
        }

        [TestMethod]
        public void test_can_read_value()
        {
            //Arrange
            socket.Setup(x => x.Receive(It.IsAny<byte[]>())).Returns(new byte[] { 0x7D, 0x04, 0x85, 0x41, 0x20, 0x00, 0x00, 0xE0 });

            //Act
            var value = cliente.GetValorEnergia();

            //Assert
            Assert.AreEqual((float)10, value);
        }

    }
}