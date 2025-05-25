using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.Lots;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.Services.Interface
{
    public interface ILotService
    {
        


        
            Task<Services.Response<Lot>> GetLotByLotCode(string LotCode);
            Task<Services.Response<LotResponse>> UpdateLot(string LotCode,LotRequest Lot);
            Task<Services.Response<LotResponse>> CreateLot(LotRequest Lot);
            Task<Services.Response<List<Lot>>> GetLotList();        
    
}
}
