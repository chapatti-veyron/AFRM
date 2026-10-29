namespace AFRM.Core
{
    public static class StatusRules
    {
        public static FuelStatus GetStatus(decimal leftoverFuelKg, decimal bufferKg)
        {
            if (leftoverFuelKg < 0)
            {
                return FuelStatus.INSUFFICIENT_FUEL;
            }

            if (leftoverFuelKg < bufferKg)
            {
                return FuelStatus.MARGINAL;
            }

            return FuelStatus.OK;
        }
    }
}
