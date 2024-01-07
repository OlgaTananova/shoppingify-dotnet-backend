using Microsoft.EntityFrameworkCore;
using shoppingify_backend.Helpers.CustomExceptions;
using shoppingify_backend.Models;
using shoppingify_backend.Models.Entities;
using shoppingify_backend.Models.ResponseModels;
using shoppingify_backend.Models.ValidationModels;
using System.Diagnostics.SymbolStore;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace shoppingify_backend.Services
{
    public interface IShoppingListService
    {
        Task<AddShoppingListDTO> CreateShoppingList(ShoppingListModel shoppingListData);
        Task<List<ShoppingListDTO>> GetShoppingLists();

        Task<DeleteShoppingListDTO> DeleteShoppingList(string slId);

        Task<UpdateShoppingListDTO> UpdateShoppingListStatus(UpdateShoppingListStatusModel updatedShoppingList);
        Task<UpdateShoppingListDTO> UpdateShoppingListHeading(UpdateShoppingListHeadingModel updatedShoppingList);

        Task<UpdateShoppingListDTO> UpdateShoppingListSalesTax(UpdateShoppingListSalesTaxModel updatedShoppingList);

    }
    public class ShoppingListService : IShoppingListService
    {
        private readonly ApplicationContext _context;
        private readonly IUserResolverService _userResolverService;

        public ShoppingListService(ApplicationContext context, IUserResolverService userResolverService)
        {
            _context = context;
            _userResolverService = userResolverService;
        }
        public async Task<AddShoppingListDTO> CreateShoppingList(ShoppingListModel shoppingListData)
        {
            string userId = _userResolverService.GetCurrentUserId();

            // If there is the active shopping list - throw exception
            var activeShL = _context.ShoppingLists.FirstOrDefaultAsync(sl => sl.Status == ShoppingListStatus.Active);

            if (activeShL.Result != null)
            {
                throw new BadRequestException("The active shopping list already exists.");
            }
            // Parser ids
            bool parsedUserId = Guid.TryParse(userId, out var userIdGuid);
            bool parsedItemId = Guid.TryParse(shoppingListData.ItemId, out var itemIdGuid);
            bool parsedCategoryId = Guid.TryParse(shoppingListData.CategoryId, out var categoryIdGuid);

            if (!parsedCategoryId || !parsedItemId || !parsedUserId)
            {
                throw new BadHttpRequestException("Failed to parse userId or/and itemId or/and categoryId");
            }

            //Find the category and item

            var addedCategory = await _context.Categories.FindAsync(categoryIdGuid);
            if (addedCategory == null)
            {
                throw new NotFoundException($"Cannot find the category with {shoppingListData.CategoryId}");
            }

            var addedItem = await _context.Items.FindAsync(itemIdGuid);
            if (addedItem == null)
            {
                throw new NotFoundException($"Cannot find the item with {shoppingListData.ItemId},");
            }

            // Create a new shopping list and shopping list item
            ShoppingList newShL = new ShoppingList
            {
                OwnerId = userIdGuid,
            };

            ShoppingListItem newShLI = new ShoppingListItem
            {
                ShoppingListId = newShL.Id,
                ShoppingList = newShL,
                Item = addedItem,
                Category = addedCategory,
                CategoryId = categoryIdGuid,
                ItemId = itemIdGuid,
                OwnerId = userIdGuid
            };

            newShL.ShoppingListItems.Add(newShLI);
            _context.ShoppingLists.Add(newShL);
            var successfullyCreated = await _context.SaveChangesAsync();

            if (successfullyCreated <= 0)
            {
                throw new BadRequestException("Failed to create a new shopping list and add the item.");
            }

            List<ShoppingListItemDTO> itemsInShL = new List<ShoppingListItemDTO>
            {
                new ShoppingListItemDTO
                {
                    _id = newShLI.Id.ToString().ToLower(),
                    ItemId = newShLI.ItemId.ToString(),
                    CategoryId = newShLI.CategoryId.ToString(),
                    Quantity = newShLI.Quantity,
                    Status = newShLI.Status.ToString().ToLower(),
                    Units = newShLI.Units,
                    PricePerUnit = newShLI.PricePerUnit,
                    Price = newShLI.Price,
                    IsDeleted = newShLI.IsDeleted
                }
            };

            var result = new AddShoppingListDTO
            {
                Message = "The new shopping list was successfully created.",
                AddedShoppingList = new ShoppingListDTO
                {
                    _id = newShL.Id.ToString(),
                    Heading = newShL.Heading,
                    Date = newShL.Date.ToLongDateString(),
                    Owner = newShL.OwnerId.ToString(),
                    Status = newShL.Status.ToString().ToLower(),
                    SalesTax = newShL.SalesTax,
                    Items = itemsInShL
                }
            };

            return result;
        }
        public Task<List<ShoppingListDTO>> GetShoppingLists()
        {
            string userId = _userResolverService.GetCurrentUserId();

            var result = _context.ShoppingLists.Where(sl => sl.IsDeleted == false).Include(i => i.ShoppingListItems).Select(sl => new ShoppingListDTO
            {
                _id = sl.Id.ToString().ToLower(),
                Heading = sl.Heading,
                Date = sl.Date.ToLongDateString(),
                Owner = sl.OwnerId.ToString().ToLower(),
                Status = sl.Status.ToString().ToLower(),
                SalesTax = sl.SalesTax,
                Items = sl.ShoppingListItems.Where(sli => sli.IsDeleted == false).Select(sli => new ShoppingListItemDTO
                {
                    _id = sli.Id.ToString().ToLower(),
                    ItemId = sli.ItemId.ToString().ToLower(),
                    CategoryId = sli.CategoryId.ToString().ToLower(),
                    Quantity = sli.Quantity,
                    Status = sli.Status.ToString().ToLower(),
                    Units = sli.Units,
                    PricePerUnit = sli.PricePerUnit,
                    Price = sli.Price,
                    IsDeleted = sli.IsDeleted,
                }).ToList()
            }).ToListAsync();

            return result;

        }

        public async Task<DeleteShoppingListDTO> DeleteShoppingList(string slId)
        {
            string userId = _userResolverService.GetCurrentUserId();

            //Parse the shopping list id
            if (!Guid.TryParse(slId, out Guid slIdGuid))
            {
                throw new BadRequestException("Failed to parse the shopping list id.");
            }

            //Find the deleted shopping list
            var deletedShL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).FirstOrDefaultAsync(s => s.Id == slIdGuid);

            if (deletedShL == null)
            {
                throw new NotFoundException($"Failed to find the shopping list with {slId} id.");
            }

            // Mark it deleted, mark the shopping list items in the list deleted
            deletedShL.IsDeleted = true;
            deletedShL.Status = ShoppingListStatus.Deleted;

            foreach (var sli in deletedShL.ShoppingListItems)
            {
                sli.IsDeleted = true;
            }
            _context.ShoppingLists.Update(deletedShL);
            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException($"Failed to delete the shopping list with {slId} id.");
            }

            // Query for remaining shopping lists
            var remainingLists = await _context.ShoppingLists
                                               .Include(sl => sl.ShoppingListItems)
                                               .Where(sl => !sl.IsDeleted)
                                               .Select(sl => new ShoppingListDTO
                                               {
                                                   _id = sl.Id.ToString().ToLower(),
                                                   Heading = sl.Heading,
                                                   Date = sl.Date.ToLongDateString(),
                                                   Owner = sl.OwnerId.ToString().ToLower(),
                                                   Status = sl.Status.ToString().ToLower(),
                                                   SalesTax = sl.SalesTax,
                                                   Items = sl.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                                                   {
                                                       _id = sli.Id.ToString().ToLower(),
                                                       ItemId = sli.ItemId.ToString().ToLower(),
                                                       CategoryId = sli.CategoryId.ToString().ToLower(),
                                                       Status = sli.Status.ToString().ToLower(),
                                                       Price = sli.Price,
                                                       PricePerUnit = sli.PricePerUnit,
                                                       Units = sli.Units,
                                                       Quantity = sli.Quantity,
                                                       IsDeleted = sli.IsDeleted
                                                   }).ToList()
                                               })
                                               .ToListAsync();
            return new DeleteShoppingListDTO
            {
                Message = "The shopping list was successfully deleted",
                DeletedShoppingList = new ShoppingListDTO
                {
                    _id = deletedShL.Id.ToString().ToLower(),
                    Heading = deletedShL.Heading,
                    Date = deletedShL.Date.ToLongDateString(),
                    Owner = deletedShL.OwnerId.ToString().ToLower(),
                    Status = deletedShL.Status.ToString().ToLower(),
                    SalesTax = deletedShL.SalesTax,
                    Items = deletedShL.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                    {
                        _id = sli.Id.ToString().ToLower(),
                        ItemId = sli.ItemId.ToString().ToLower(),
                        CategoryId = sli.CategoryId.ToString().ToLower(),
                        Status = sli.Status.ToString().ToLower(),
                        Price = sli.Price,
                        PricePerUnit = sli.PricePerUnit,
                        Units = sli.Units,
                        Quantity = sli.Quantity,
                        IsDeleted = sli.IsDeleted
                    }).ToList()

                },
                RemainingShoppingLists = remainingLists
            };
        }

        public async Task<UpdateShoppingListDTO> UpdateShoppingListStatus(UpdateShoppingListStatusModel updatedShoppingList)
        {
            // Parse id
            if (!Guid.TryParse(updatedShoppingList.ShoppingListId, out Guid guidSlId))
            {
                throw new BadRequestException("Cannot parse the shopping list's id");

            }

            // Validate status
            string capitalizedFirstLetterStatus = updatedShoppingList.Status.Substring(0, 1).ToUpper() + updatedShoppingList.Status.Substring(1);

            if (!Enum.TryParse(capitalizedFirstLetterStatus, out ShoppingListStatus status))
            {
                throw new BadRequestException("Wrong status of the shopping list");
            }

            // find the shopping list by id and update it status

            var updatedSL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).FirstOrDefaultAsync(sl => sl.Id == guidSlId && sl.Status == ShoppingListStatus.Active);
            if (updatedSL == null)
            {
                throw new NotFoundException($"The shopping list with {updatedShoppingList.ShoppingListId} id was not found.");
            }

            updatedSL.Status = status;
            _context.ShoppingLists.Update(updatedSL);
            var result = await _context.SaveChangesAsync();
            if (result <= 0)
            {
                throw new BadRequestException("Failed to update the shopping list status");
            }
            // Get all shopping lists
            var allSL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).Where(sl => !sl.IsDeleted).Select(sl => new ShoppingListDTO
            {
                _id = sl.Id.ToString().ToLower(),
                Heading = sl.Heading,
                Owner = sl.OwnerId.ToString().ToLower(),
                Status = sl.Status.ToString().ToLower(),
                SalesTax = sl.SalesTax,
                Date = sl.Date.ToLongDateString(),
                Items = sl.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                {
                    _id = sli.Id.ToString().ToLower(),
                    CategoryId = sli.CategoryId.ToString().ToLower(),
                    Units = sli.Units,
                    Status = sli.Status.ToString(),
                    Quantity = sli.Quantity,
                    PricePerUnit = sli.PricePerUnit,
                    Price = sli.Price,
                    ItemId = sli.ItemId.ToString().ToLower(),
                    IsDeleted = sli.IsDeleted,
                }).ToList(),
            }).ToListAsync();

            return new UpdateShoppingListDTO
            {
                Message = "The shopping list status was successfully updated.",
                UpdatedShoppingList = new ShoppingListDTO
                {
                    _id = updatedSL.Id.ToString().ToLower(),
                    Heading = updatedSL.Heading,
                    Date = updatedSL.Date.ToLongDateString(),
                    Owner = updatedSL.OwnerId.ToString().ToLower(),
                    Status = updatedSL.Status.ToString().ToLower(),
                    SalesTax = updatedSL.SalesTax,
                    Items = updatedSL.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                    {
                        _id = sli.Id.ToString().ToLower(),
                        ItemId = sli.ItemId.ToString().ToLower(),
                        CategoryId = sli.CategoryId.ToString().ToLower(),
                        Status = sli.Status.ToString().ToLower(),
                        Price = sli.Price,
                        PricePerUnit = sli.PricePerUnit,
                        Units = sli.Units,
                        Quantity = sli.Quantity,
                        IsDeleted = sli.IsDeleted
                    }).ToList()
                },
                AllShoppingLists = allSL
            };

        }

        public async Task<UpdateShoppingListDTO> UpdateShoppingListHeading(UpdateShoppingListHeadingModel updatedShoppingList)
        {
            // Parse id
            if (!Guid.TryParse(updatedShoppingList.ShoppingListId, out Guid guidSlId))
            {
                throw new BadRequestException("Cannot parse the shopping list's id");

            }

            // Find the shopping list by id and update it heading
            var updatedSL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).FirstOrDefaultAsync(sl => sl.Id == guidSlId && sl.Status == ShoppingListStatus.Active);

            if (updatedSL == null)
            {
                throw new NotFoundException($"The shopping list with {updatedShoppingList.ShoppingListId} id was not found.");
            }

            updatedSL.Heading = updatedShoppingList.Heading;
            _context.ShoppingLists.Update(updatedSL);
            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("Failed to update the shopping list heading");
            }

            // Get all shopping lists
            var allSL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).Where(sl => !sl.IsDeleted).Select(sl => new ShoppingListDTO
            {
                _id = sl.Id.ToString().ToLower(),
                Heading = sl.Heading,
                Owner = sl.OwnerId.ToString().ToLower(),
                Status = sl.Status.ToString().ToLower(),
                SalesTax = sl.SalesTax,
                Date = sl.Date.ToLongDateString(),
                Items = sl.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                {
                    _id = sli.Id.ToString().ToLower(),
                    CategoryId = sli.CategoryId.ToString().ToLower(),
                    Units = sli.Units,
                    Status = sli.Status.ToString(),
                    Quantity = sli.Quantity,
                    PricePerUnit = sli.PricePerUnit,
                    Price = sli.Price,
                    ItemId = sli.ItemId.ToString().ToLower(),
                    IsDeleted = sli.IsDeleted,
                }).ToList(),
            }).ToListAsync();

            return new UpdateShoppingListDTO
            {
                Message = "The shopping list heading was successfully updated.",
                UpdatedShoppingList = new ShoppingListDTO
                {
                    _id = updatedSL.Id.ToString().ToLower(),
                    Heading = updatedSL.Heading,
                    Date = updatedSL.Date.ToLongDateString(),
                    Owner = updatedSL.OwnerId.ToString().ToLower(),
                    Status = updatedSL.Status.ToString().ToLower(),
                    SalesTax = updatedSL.SalesTax,
                    Items = updatedSL.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                    {
                        _id = sli.Id.ToString().ToLower(),
                        ItemId = sli.ItemId.ToString().ToLower(),
                        CategoryId = sli.CategoryId.ToString().ToLower(),
                        Status = sli.Status.ToString().ToLower(),
                        Price = sli.Price,
                        PricePerUnit = sli.PricePerUnit,
                        Units = sli.Units,
                        Quantity = sli.Quantity,
                        IsDeleted = sli.IsDeleted
                    }).ToList()
                },
                AllShoppingLists = allSL
            };
        }
        public async Task<UpdateShoppingListDTO> UpdateShoppingListSalesTax(UpdateShoppingListSalesTaxModel updatedShoppingList)
        {
            // Parse id
            if (!Guid.TryParse(updatedShoppingList.ShoppingListId, out Guid guidSlId))
            {
                throw new BadRequestException("Cannot parse the shopping list's id");

            }

            // Find the shopping list by id and update it heading
            var updatedSL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).FirstOrDefaultAsync(sl => sl.Id == guidSlId && sl.Status == ShoppingListStatus.Active);

            if (updatedSL == null)
            {
                throw new NotFoundException($"The shopping list with {updatedShoppingList.ShoppingListId} id was not found.");
            }

            updatedSL.SalesTax = updatedShoppingList.SalesTax;
            _context.ShoppingLists.Update(updatedSL);
            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new BadRequestException("Failed to update the shopping list sales tax.");
            }

            // Get all shopping lists
            var allSL = await _context.ShoppingLists.Include(sl => sl.ShoppingListItems).Where(sl => !sl.IsDeleted).Select(sl => new ShoppingListDTO
            {
                _id = sl.Id.ToString().ToLower(),
                Heading = sl.Heading,
                Owner = sl.OwnerId.ToString().ToLower(),
                Status = sl.Status.ToString().ToLower(),
                SalesTax = sl.SalesTax,
                Date = sl.Date.ToLongDateString(),
                Items = sl.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                {
                    _id = sli.Id.ToString().ToLower(),
                    CategoryId = sli.CategoryId.ToString().ToLower(),
                    Units = sli.Units,
                    Status = sli.Status.ToString(),
                    Quantity = sli.Quantity,
                    PricePerUnit = sli.PricePerUnit,
                    Price = sli.Price,
                    ItemId = sli.ItemId.ToString().ToLower(),
                    IsDeleted = sli.IsDeleted,
                }).ToList(),
            }).ToListAsync();

            return new UpdateShoppingListDTO
            {
                Message = "The shopping list sales tax was successfully updated.",
                UpdatedShoppingList = new ShoppingListDTO
                {
                    _id = updatedSL.Id.ToString().ToLower(),
                    Heading = updatedSL.Heading,
                    Date = updatedSL.Date.ToLongDateString(),
                    Owner = updatedSL.OwnerId.ToString().ToLower(),
                    Status = updatedSL.Status.ToString().ToLower(),
                    SalesTax = updatedSL.SalesTax,
                    Items = updatedSL.ShoppingListItems.Select(sli => new ShoppingListItemDTO
                    {
                        _id = sli.Id.ToString().ToLower(),
                        ItemId = sli.ItemId.ToString().ToLower(),
                        CategoryId = sli.CategoryId.ToString().ToLower(),
                        Status = sli.Status.ToString().ToLower(),
                        Price = sli.Price,
                        PricePerUnit = sli.PricePerUnit,
                        Units = sli.Units,
                        Quantity = sli.Quantity,
                        IsDeleted = sli.IsDeleted
                    }).ToList()
                },
                AllShoppingLists = allSL
            };
        }

    }
}