using DefectListDomain.Models;

namespace DefectListDomain.Services
{
    public interface IMeasurementMapItemPresenterService
    {
        MeasurementMapItemDisplayModel Present(MeasurementMapItem item);
    }
}