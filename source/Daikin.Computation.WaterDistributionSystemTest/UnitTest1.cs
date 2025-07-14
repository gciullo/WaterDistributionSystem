using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daikin.Computation.WaterDistributionSystem;
using Daikin.Computation.WaterDistributionSystem.Components.SchemaModel;
using Daikin.Computation.WaterDistributionSystem.DetectData;
using Daikin.Computation.WaterDistributionSystem.Components.GAModel;

namespace Daikin.Computation.WaterDistributionSystem;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {      

        TestInput testInput = new TestInput();

        var input = testInput.GenerateInput1();
        WaterDistributionSystem waterDistributionSystem = new WaterDistributionSystem();

        waterDistributionSystem.Init(input);

        WaterDistributionSystemInputData waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0
        };
       
        CancellationToken cancellationToken = new CancellationToken();
        DateTime dateTime = DateTime.Now;
        var result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        DateTime dateTime1 = DateTime.Now;

        var time = dateTime1 - dateTime;
        waterDistributionSystem.Init(input);
        waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0
        };

         cancellationToken= new CancellationToken();
        dateTime = DateTime.Now;
        result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        dateTime = DateTime.Now;

        time = dateTime1 - dateTime;

        waterDistributionSystem.Init(input);

        waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0
        };

         cancellationToken = new CancellationToken();
         dateTime = DateTime.Now;
         result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
         dateTime1 = DateTime.Now;

         time = dateTime1 - dateTime;
    }
    
    [TestMethod]
    public void Test_SixEqualFancoils()
    {      

        TestInput testInput = new TestInput();

        var input = testInput.GenerateInput2();
        WaterDistributionSystem waterDistributionSystem = new WaterDistributionSystem();

        waterDistributionSystem.Init(input);

        WaterDistributionSystemInputData waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0
        };
       
        CancellationToken cancellationToken = new CancellationToken();
        DateTime dateTime = DateTime.Now;
        var result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        DateTime dateTime1 = DateTime.Now;

        var time = dateTime1 - dateTime;
    }
    
    [TestMethod]
    public void Test_Fancoils_WithoutRooms()
    {      

        TestInput testInput = new TestInput();

        var input = testInput.GenerateInput3();
        WaterDistributionSystem waterDistributionSystem = new WaterDistributionSystem();

        waterDistributionSystem.Init(input);

        WaterDistributionSystemInputData waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0
        };
       
        CancellationToken cancellationToken = new CancellationToken();
        DateTime dateTime = DateTime.Now;
        var result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        DateTime dateTime1 = DateTime.Now;

        var time = dateTime1 - dateTime;
    }
    
    [TestMethod]
    public void Test_Fancoils_WithUnbalancedCapacity()
    {      

        TestInput testInput = new TestInput();

        var input = testInput.GenerateInputFanCoilsWithUnbalancedCapacity();
        WaterDistributionSystem waterDistributionSystem = new WaterDistributionSystem();

        waterDistributionSystem.Init(input);

        WaterDistributionSystemInputData waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0,
            AllowSwitchOffUnit = false
        };
       
        CancellationToken cancellationToken = new CancellationToken();
        DateTime dateTime = DateTime.Now;
        var result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        DateTime dateTime1 = DateTime.Now;

        var time = dateTime1 - dateTime;
    }
    
    [TestMethod]
    public void Test_Fancoils_WithUnbalancedCapacity2()
    {      

        TestInput testInput = new TestInput();

        var input = testInput.GenerateInputFanCoilsWithUnbalancedCapacity2();
        WaterDistributionSystem waterDistributionSystem = new WaterDistributionSystem();

        waterDistributionSystem.Init(input);

        WaterDistributionSystemInputData waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0,
            AllowSwitchOffUnit = false,
        };
       
        CancellationToken cancellationToken = new CancellationToken();
        DateTime dateTime = DateTime.Now;
        var result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        DateTime dateTime1 = DateTime.Now;

        var time = dateTime1 - dateTime;
    }
    
    [TestMethod]
    public void Test_TenEqualFancoils()
    {
        TestInput testInput = new TestInput();

        var input = testInput.GenerateInput10FanCoils();
        WaterDistributionSystem waterDistributionSystem = new WaterDistributionSystem();

        waterDistributionSystem.Init(input);

        WaterDistributionSystemInputData waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0
        };
       
        CancellationToken cancellationToken = new CancellationToken();
        DateTime dateTime = DateTime.Now;
        var result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        DateTime dateTime1 = DateTime.Now;

        var time = dateTime1 - dateTime;
    }
    
    [TestMethod]
    public void Test_FiftyEqualFancoils()
    {      
        TestInput testInput = new TestInput();

        var input = testInput.GenerateInput50FanCoils();
        WaterDistributionSystem waterDistributionSystem = new WaterDistributionSystem();

        waterDistributionSystem.Init(input);

        WaterDistributionSystemInputData waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0
        };
       
        CancellationToken cancellationToken = new CancellationToken();
        DateTime dateTime = DateTime.Now;
        var result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        DateTime dateTime1 = DateTime.Now;

        var time = dateTime1 - dateTime;
    }
    
    [TestMethod]
    public void Test_OneHundredEqualFancoils()
    {      
        TestInput testInput = new TestInput();

        var input = testInput.GenerateInput100FanCoils();
        WaterDistributionSystem waterDistributionSystem = new WaterDistributionSystem();

        waterDistributionSystem.Init(input);

        WaterDistributionSystemInputData waterDistributionSystemInputData = new WaterDistributionSystemInputData
        {
            FluidType = EnumData.FluidType.WATER,
            WaterInletTemperature = 7,
            GlycolPercentage = 0
        };
       
        CancellationToken cancellationToken = new CancellationToken();
        DateTime dateTime = DateTime.Now;
        var result = waterDistributionSystem.Detect(waterDistributionSystemInputData, cancellationToken).Result;
        DateTime dateTime1 = DateTime.Now;

        var time = dateTime1 - dateTime;
    }
}
