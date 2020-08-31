using Library.Models.Catalog;
using Library.Models.CheckoutModels;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _assets;
        private ICheckout _checkoutsService;
        public CatalogController(ILibraryAsset assets, ICheckout checkouts)
        {
            _assets = assets;
            _checkoutsService = checkouts;
        }

        public IActionResult Index(string searching)
        {
            var assetModels = _assets.GetAll();
            if (searching != null)
            {
                assetModels = assetModels.Where(x => x.Title.ToLower().Contains(searching.ToLower()));
            }
            var listingResult = assetModels.Select(result => new AssetIndexListingModel
            {
                Id = result.Id,
                ImageUrl = result.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                DeweyCallNumber = _assets.GetDeweyIndex(result.Id),
                Title = result.Title,
                Type = _assets.GetType(result.Id) 
            });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };

            return View(model);
        }
        public IActionResult Detail(int id)
        {
            var asset = _assets.GetById(id);

            var currentHolds = _checkoutsService.GetCurrentHolds(id)
                .Select(a => new AssetHoldModel 
                {
                    HoldPlaced = _checkoutsService.GetCurrentHoldPlaced(a.Id),
                    PatronName = _checkoutsService.GetCurrentHoldPatronName(a.Id)
                });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assets.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                CheckoutHistory = _checkoutsService.GetCheckoutHistory(id),
                ISBN = _assets.GetIsbn(id),
                LatestCheckout = _checkoutsService.GetLatestCheckout(id),
                PatronName = _checkoutsService.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds
            };
            return View(model);
        }

        public  IActionResult CheckOut(int id)
        {
            var asset = _assets.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkoutsService.IsCheckedOut(id)
            };

            return View(model);
        }
        
        public IActionResult CheckIn(int id)
        {
            _checkoutsService.CheckInItem(id);
            return RedirectToAction("Detail", new { id = id });
        }

        public IActionResult MarkLost(int assetId)
        {
            _checkoutsService.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }
        public IActionResult MarkFound(int assetId)
        {
            _checkoutsService.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            _checkoutsService.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkoutsService.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult Hold(int id)
        {
            var asset = _assets.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkoutsService.IsCheckedOut(id),
                HoldCount = _checkoutsService.GetCurrentHolds(id).Count()
            };

            return View(model);
        }

    }
}
