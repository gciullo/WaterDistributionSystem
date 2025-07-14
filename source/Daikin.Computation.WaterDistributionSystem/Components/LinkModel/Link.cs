using Daikin.Computation.WaterDistributionSystem.Components.ComponentModel;
using Daikin.Computation.WaterDistributionSystem.Interfaces;
using Daikin.Computation.WaterDistributionSystem.Components.PipeModel;


namespace Daikin.Computation.WaterDistributionSystem.Components.LinkModel;

public class Link : Component, IDetectHandler<LinkInputData, LinkOutputData>
{
  
    public Link()
    {        
    }
    #region Properties    
    public Guid ToId { get; set; }
    public Guid FromId { get; set; }
    public Components.PipeModel.Pipe Pipe { get; set; }
    #endregion  

    public async Task<LinkOutputData> Detect(LinkInputData input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        LinkOutputData outputData= new LinkOutputData();
        PipeInputData pipeInputData = new PipeInputData
        {
            WaterFlow = input.WaterFlow
            
        };
        var pipeOutput = await Pipe.Detect(pipeInputData, cancellationToken);
        
        outputData.WaterFlow = input.WaterFlow;
        outputData.PressureDrop = pipeOutput.PressureDrop;


        return outputData;
    }   
}
