namespace BlazamSetup.Steps
{
    internal interface IInstallationStep
    {
         IInstallationStep NextStep();
    }
}