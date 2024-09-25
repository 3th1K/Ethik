using Microsoft.EntityFrameworkCore;

namespace Ethik.Utility.Data.Repository;

public static class RepositoryErrorCodes
{
    public const string ConnectionTimeout = "connection_timeout";
    public const string SoftDeleteNotSupported = "soft_delete_not_supported";

    public const string EntityNotFound = "entity_not_found";
    public const string AddEntityFailure = "entity_add_failed";
    public const string DeleteEntityFailure = "entity_delete_failed";
    public const string SoftDeleteEntityFailure = "entity_soft_delete_failed";
    public const string CheckEntityExistsFailure = "entity_existance_check_failed";
    public const string FetchEntityFailure = "entity_fetch_failed";
    public const string UpdateEntityFailure = "entity_update_failed";

    public const string EntitiesNotFound = "entities_not_found";
    public const string AddEntitiesFailure = "entities_add_failed";
    public const string DeleteEntitiesFailure = "entities_delete_failed";
    public const string CountEntitiesFailure = "entities_count_failed";
    public const string FindEntitiesFailure = "entities_find_failed";
    public const string FetchAllEntitiesFailure = "entities_fetch_all_failed";
    public const string UpdateEntitiesFailure = "entities_update_failed";
}