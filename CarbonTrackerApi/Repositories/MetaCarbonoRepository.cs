using CarbonTrackerApi.Data;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Repositories;

public class MetaCarbonoRepository(ApplicationDbContext context)
    : Repository<MetaCarbono>(context), IMetaCarbonoRepository
{
    
}