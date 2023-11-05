Imports System.IO
Imports System.Text
Imports Int = System.Int32
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Primitives

Imports SPCMS.Models
Imports SPCMS.Models.Tables

Namespace Controllers
    Public Class HomeController : Inherits Controller
        Private ReadOnly context As New SparePartsContext()
        Private ReadOnly repository As New SparePartsRepository()
        Private ReadOnly hostEnv As IWebHostEnvironment

        Private spare As SpareParts
        Private Property PreservedListings As List(Of SpareParts)
        Private tempTable As TableContext

        Private create, deleted
        Private Shadows response As String

        Public Sub New(hostenv As IWebHostEnvironment)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
            Me.hostEnv = hostenv
        End Sub

        <HttpGet(), Route("")>
        Public Function Index() As IActionResult
            tempTable = Table
            Table = TableContext.SPAREPARTS
            repository.GetAllListings()

            If Not String.IsNullOrWhiteSpace(Session.User) AndAlso (Session.Role = Session.Roles.SPARE_PARTS_DEALER Or Session.Role = Session.Roles.ADMIN) Then
                Dim lowStock As List(Of String) = repository.SpareList.Where(Function(r As SpareParts) Val(r.Quantity) <= Val(r.QuantityRequired) And r.Username = Session.User).Select(Function(s As SpareParts) s.ID).ToList()
                ViewData!CriticalCount = lowStock.Count
                ViewData!lowStock = lowStock
                REM Console.WriteLine(String.Join(vbLf, From r In repository.SpareList Select New With {r.Name, r.Quantity, r.QuantityRequired}))
            End If
            ViewData!wwwroot = hostEnv.WebRootPath
            Table = tempTable
            Return MyBase.View(IndexView.CreateNew(ViewData, repository.SpareList), repository.SpareList)
        End Function

        <HttpGet(), Route("Privacy")>
        Public Function Privacy() As IActionResult
            Return MyBase.View()
        End Function

        Private Sub Paginate(page As Int, limit As Int)
            Const maxBtnsPerView As Int = 10
            Dim offset As Int = page * limit - limit,
                total_rows As Int = repository.SpareList.Count,
                total_pages As Int = Math.Ceiling(total_rows / limit),
                [next] As Int = page + 1, prev = page - 1,
                from As Int = page - (page Mod maxBtnsPerView),
                [to] As Int = Math.Ceiling(page / maxBtnsPerView) * maxBtnsPerView

            If page Mod maxBtnsPerView = 0 Then
                [to] += maxBtnsPerView
            End If
            If maxBtnsPerView > total_pages Then
                [to] = total_pages + 1
            End If
            repository.SpareList = repository.SpareList.Skip(offset).Take(limit).ToList()

            ViewData!page = page
            ViewData!next = [next]
            ViewData!prev = prev
            ViewData!total_pages = total_pages
            ViewData!total_rows = total_rows
            ViewData!from = from
            ViewData!to = [to]
            ViewData!per = maxBtnsPerView
        End Sub

        Private Sub Filter(Optional make As String() = Nothing, Optional model As String() = Nothing,
                           Optional minYear As String = Nothing, Optional maxYear As String = Nothing,
                           Optional category As String() = Nothing, Optional minPrice As String = Nothing,
                           Optional maxPrice As String = Nothing, Optional brand As String() = Nothing,
                           Optional colour As String() = Nothing, Optional material As String() = Nothing,
                           Optional seller As String() = Nothing)
            If PreservedListings IsNot Nothing Then
                For Each s In repository.SpareList.Select(Function(val As SpareParts, i As Int) New With {i, val})
                    Console.WriteLine("REPO BEFORE {0}: KShs. {1} Year {2}", s.i, s.val.Price, s.val.YearOfManufacture)
                Next s

                If make.Length > 0 Then
                    repository.SpareList = (From r In repository.SpareList Join m As String In make On r.VehicleMake Equals m Select r).ToList()
                    Console.WriteLine("COUNT 1: " & repository.SpareList.Count)
                End If
                If model.Length > 0 Then
                    repository.SpareList = (From r In repository.SpareList Join m As String In model On r.VehicleModel Equals m Select r).ToList()
                    Console.WriteLine("COUNT 2: " & repository.SpareList.Count)
                End If
                If minYear IsNot Nothing AndAlso maxYear IsNot Nothing Then
                    repository.SpareList = repository.SpareList.Where(Function(r As SpareParts) Val(r.YearOfManufacture) >= Val(minYear) And Val(r.YearOfManufacture) <= Val(maxYear)).ToList()
                    Console.WriteLine("COUNT 3: " & repository.SpareList.Count)
                End If
                If category.Length > 0 Then
                    repository.SpareList = (From r In repository.SpareList Join c As String In category On r.Category Equals c Select r).ToList()
                    Console.WriteLine("COUNT 4: " & repository.SpareList.Count)
                End If
                If minPrice IsNot Nothing AndAlso maxPrice IsNot Nothing Then
                    Console.WriteLine("MIN: {0}{1}MAX: {2}", minPrice, vbLf, maxPrice)
                    repository.SpareList = repository.SpareList.Where(Function(r As SpareParts) Val(r.Price) >= Val(minPrice) And Val(r.Price) <= Val(maxPrice)).ToList()
                    Console.WriteLine("COUNT 5: " & repository.SpareList.Count)
                End If
                If brand.Length > 0 Then
                    repository.SpareList = (From r In repository.SpareList Join b As String In brand On r.Brand Equals b Select r).ToList()
                    Console.WriteLine("COUNT 6: " & repository.SpareList.Count)
                End If
                If colour.Length > 0 Then
                    repository.SpareList = (From r In repository.SpareList Join c As String In colour On r.Colour Equals c Select r).ToList()
                    Console.WriteLine("COUNT 7: " & repository.SpareList.Count)
                End If
                If material.Length > 0 Then
                    repository.SpareList = (From r In repository.SpareList Join m As String In material On r.Material Equals m Select r).ToList()
                    Console.WriteLine("COUNT 8: " & repository.SpareList.Count)
                End If
                If seller.Length > 0 Then
                    repository.SpareList = (From r In repository.SpareList Join s As String In seller On r.Username Equals s Select r).ToList()
                    Console.WriteLine("COUNT 9: " & repository.SpareList.Count)
                End If
                ViewData!total_rows = repository.SpareList.Count
            End If
            For Each s In repository.SpareList.Select(Function(val, i) New With {i, val})
                Console.WriteLine("REPO AFTER {0}: KShs. {1} Year {2}", s.i, s.val.Price, s.val.YearOfManufacture)
            Next s
        End Sub

        <HttpGet("{page}/{make?}/{model?}/{minYear?}/{maxYear?}/{category?}/{minPrice?}/{maxPrice?}/{brand?}/{colour?}/{material?}/{seller?}/{limit?}"), Route("Search")>
        Public Function Search(query As String, page As Int, Optional make As String() = Nothing,
                               Optional model As String() = Nothing, Optional minYear As String = Nothing,
                               Optional maxYear As String = Nothing, Optional category As String() = Nothing,
                               Optional minPrice As String = Nothing, Optional maxPrice As String = Nothing,
                               Optional brand As String() = Nothing, Optional colour As String() = Nothing,
                               Optional material As String() = Nothing, Optional seller As String() = Nothing,
                               Optional limit As Int = 5) As IActionResult
            tempTable = Table
            Table = TableContext.SEARCH
            repository.GetAllListings(query)
            ViewData!query = query
            ViewData!original = repository.SpareList
            PreservedListings = repository.SpareList
            ViewData!getStr = String.Join(";"c, HttpContext.Request.Query.Select(Function(q As KeyValuePair(Of String, StringValues)) String.Format("{0}:{1}", q.Key, q.Value)))

            Dim sortByPrice As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.Price)).Select(Function(r As SpareParts) r.Price)
            If sortByPrice.Any() Then
                ViewData!upperPrice = sortByPrice.First()
                ViewData!lowerPrice = sortByPrice.Last()
            End If
            Dim sortByYear As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.YearOfManufacture)).Select(Function(r As SpareParts) r.YearOfManufacture)
            If sortByYear.Any() Then
                ViewData!upperYear = sortByYear.First()
                ViewData!lowerYear = sortByYear.Last()
            End If

            Filter(make, model, minYear, maxYear, category, minPrice, maxPrice, brand, colour, material, seller)
            Paginate(page, limit)
            Table = tempTable
            Return MyBase.View(SearchView.CreateNew(repository.SpareList, ViewData), ViewData)
        End Function

        <HttpGet("{page}/{make?}/{model?}/{minYear?}/{maxYear?}/{category?}/{minPrice?}/{maxPrice?}/{brand?}/{colour?}/{material?}/{seller?}/{limit?}"), Route("Listings")>
        Public Function Listings(page As Int, Optional make As String() = Nothing,
                                 Optional model As String() = Nothing, Optional minYear As String = Nothing,
                                 Optional maxYear As String = Nothing, Optional category As String() = Nothing,
                                 Optional minPrice As String = Nothing, Optional maxPrice As String = Nothing,
                                 Optional brand As String() = Nothing, Optional colour As String() = Nothing,
                                 Optional material As String() = Nothing, Optional seller As String() = Nothing,
                                 Optional limit As Int = 5) As IActionResult
            tempTable = Table
            Table = TableContext.SPAREPARTS
            repository.GetAllListings()

            ViewData!original = repository.SpareList
            PreservedListings = repository.SpareList

            ViewData!getStr = String.Join(";"c, HttpContext.Request.Query.Select(Function(q As KeyValuePair(Of String, StringValues)) String.Format("{0}:{1}", q.Key, q.Value)))

            If Not String.IsNullOrWhiteSpace(Session.User) AndAlso Session.Role = Session.Roles.SPARE_PARTS_DEALER Then
                Console.WriteLine("USER NOT NULL: {0}", Session.User)
                repository.SpareList = repository.SpareList.Where(Function(r As SpareParts) r.Username = Session.User).ToList()
            End If

            Dim sortByPrice As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.Price)).Select(Function(r As SpareParts) r.Price)
            If sortByPrice.Any() Then
                ViewData!upperPrice = sortByPrice.First()
                ViewData!lowerPrice = sortByPrice.Last()
            End If
            Dim sortByYear As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.YearOfManufacture)).Select(Function(r As SpareParts) r.YearOfManufacture)
            If sortByYear.Any() Then
                ViewData!upperYear = sortByYear.First()
                ViewData!lowerYear = sortByYear.Last()
            End If

            Filter(make, model, minYear, maxYear, category, minPrice, maxPrice, brand, colour, material, seller)
            Paginate(page, limit)
            Table = tempTable
            Return MyBase.View(ListingsView.CreateNew(repository.SpareList, ViewData), repository.SpareList)
        End Function

        <HttpGet(), Route("AddListing")>
        Public Function AddListing() As IActionResult
            Return MyBase.View(AddView.CreateNew(ViewData))
        End Function

        <HttpPost(), Route("AddListing")>
        Public Function AddListing(name As String, category As String, brand As String, id As String, username As String,
                                   quantity As String, quantityRequired As String, price As String, year As String,
                                   make As String, model As String, uploads As List(Of IFormFile), description As String,
                                   colour As String, material As String, dimensions As String) As IActionResult
            Try
                Dim reader As New StreamReader(Path.Combine(Startup.GetEnv.WebRootPath, "colors.json"))
                colour = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(reader.ReadToEnd()).Where(Function(w) w.Value = colour).FirstOrDefault.Key
                reader.Close()
                tempTable = Table
                Table = TableContext.SPAREPARTS
                Console.WriteLine($"{name}{vbLf}{category}{vbLf}{brand}{vbLf}{id}{vbLf}{username}{vbLf}{quantity}{vbLf}{quantityRequired}{vbLf}{price}{vbLf}{make}{vbLf}{model}{vbLf}{uploads}{vbLf}{description}{vbLf}{colour}{vbLf}{material}{vbLf}{dimensions}")
                context.Create(New SpareParts() With {
                    .Name = name,
                    .Category = category,
                    .Brand = brand,
                    .ID = id,
                    .Username = username,
                    .Quantity = quantity,
                    .QuantityRequired = quantityRequired,
                    .Price = price,
                    .YearOfManufacture = year,
                    .VehicleMake = make,
                    .VehicleModel = model,
                    .Uploads = HandleUpload(uploads),
                    .Description = description,
                    .Colour = colour,
                    .Material = material,
                    .Dimensions = dimensions
                })
                REM ViewData!Response = $"{name}<br>{category}<br>{brand}<br>{brand}<br>{id}<br>{make}<br>{model}<br>{quantity}"
                ViewData!Response = <div class="alert alert-success">Successful data entry</div>.ToString()
            Catch e As MySql.Data.MySqlClient.MySqlException
                If e.Number = 1062 Then
                    ViewData!response = <div class="alert alert-danger">This listing already exists.</div>.ToString()
                Else Throw
                End If
            Finally
                Table = tempTable
            End Try
            Return MyBase.View(AddView.CreateNew(viewData:=ViewData))
        End Function

        Private Function HandleUpload(uploads As List(Of IFormFile)) As String
            Dim files As New StringBuilder(), filePath As String,
                uploadDir As String = Path.Combine(hostEnv.WebRootPath, "Uploads"), fileName As String
            For Each f As IFormFile In uploads
                If f.Length > 0 Then
                    If Not Directory.Exists(uploadDir) Then
                        Directory.CreateDirectory(uploadDir)
                    End If
                    fileName = String.Format("{0}-{1}", Guid.NewGuid(), f.FileName.Replace(";"c, Nothing))
                    filePath = Path.Combine(uploadDir, fileName)
                    Using stream As New FileStream(filePath, FileMode.Create)
                        f.CopyTo(stream)
                    End Using
                    files.Append(Path.Combine("~"c, "Uploads", fileName) & ";"c)
                End If
            Next f
            Return files.ToString()
        End Function

        <HttpGet("{id}/{username}"), Route("DeleteListing")>
        Public Function DeleteListing(id As String, username As String) As String
            tempTable = Table
            Table = TableContext.SPAREPARTS
            deleted = context.Delete(New SpareParts() With {.ID = id, .Username = username}) > 0
            Table = tempTable
            Return If(deleted, <div class="alert alert-success">Successfully deleted listing.</div>.ToString(),
                <div class="alert alert-danger">An error occurred while deleting this listing.</div>.ToString())
        End Function


        <HttpGet("{id}/Details"), Route("{id}/Details")>
        Public Function Details(id As String) As IActionResult
            'If Table = TableContext.WISHLIST Then
            '    Table = TableContext.SPAREPARTS
            'End If
            repository.GetAllListings()
            Console.WriteLine("ID = {0}{1}LEN={2}", id, vbLf, repository.SpareList.Count)
            spare = repository.SpareList.First(Function(s As SpareParts) s.ID.Equals(Web.HttpUtility.UrlDecode(id)))
            Return MyBase.View(DetailsView.CreateNew(spare, ViewData))
        End Function

        <HttpGet("{id}/Edit"), Route("{id}/Edit")>
        Public Function Edit(id As String) As IActionResult
            'ViewData!Response = JObject.Parse($"{{""category"":""{category}"",""quantity"":""{quantity}"",""description"":""{description}"",""ID"":""{id}"",""price"":""{price}""}}")
            repository.GetAllListings()
            Console.WriteLine(String.Format("ID = {0}{1}LEN = {2}", id, vbLf, repository.SpareList.Count))
            Dim temp As SpareParts = repository.SpareList.First(Function(s As SpareParts) s.ID.Equals(id))
            spare = New SpareParts() With {
                .Category = temp.Category,
                .Quantity = temp.Quantity,
                .Description = temp.Description,
                .ID = temp.ID,
                .Price = temp.Price
            }
            Return MyBase.View(EditView.CreateNew(spare, ViewData))
        End Function

        <HttpPost(), Route("SaveEdits")>
        Public Function SaveEdits(category As String, quantity As String, description As String, ID As String, price As String) As IActionResult
            spare = New SpareParts() With {.Category = category, .Quantity = quantity, .Description = description, .ID = ID, .Price = price}
            'ViewData!Response = JObject.Parse($"{{""category"":""{category}"",""quantity"":""{quantity}"",""description"":""{description}"",""ID"":""{ID}"",""price"":""{price}""}}")
            ViewData!Update = If(context.Edit(spare) > 0,
                <div class="alert alert-success">Successfully updated record.</div>.ToString(),
                <div class="alert alert-danger">No such records to update for ID "<%= spare.ID %>"</div>.ToString())
            Return MyBase.View(EditView.CreateNew(spare, ViewData))
        End Function

        <HttpGet("{page}/{make?}/{model?}/{minYear?}/{maxYear?}/{category?}/{minPrice?}/{maxPrice?}/{brand?}/{colour?}/{material?}/{seller?}/{limit?}"), Route("WishList")>
        Public Function WishList(page As Int, Optional make As String() = Nothing,
                                 Optional model As String() = Nothing, Optional minYear As String = Nothing,
                                 Optional maxYear As String = Nothing, Optional category As String() = Nothing,
                                 Optional minPrice As String = Nothing, Optional maxPrice As String = Nothing,
                                 Optional brand As String() = Nothing, Optional colour As String() = Nothing,
                                 Optional material As String() = Nothing, Optional seller As String() = Nothing,
                                 Optional limit As Int = 5) As IActionResult
            tempTable = Table
            Table = TableContext.WISHLIST
            repository.GetAllListings()
            ViewData!original = repository.SpareList
            PreservedListings = repository.SpareList
            ViewData!getStr = String.Join(";"c, HttpContext.Request.Query.Select(Function(q As KeyValuePair(Of String, StringValues)) String.Format("{0}:{1}", q.Key, q.Value)))

            Dim sortByPrice As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.Price)).Select(Function(r As SpareParts) r.Price)
            If sortByPrice.Any() Then
                ViewData!upperPrice = sortByPrice.First()
                ViewData!lowerPrice = sortByPrice.Last()
            End If
            Dim sortByYear As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.YearOfManufacture)).Select(Function(r As SpareParts) r.YearOfManufacture)
            If sortByYear.Any() Then
                ViewData!upperYear = sortByYear.First()
                ViewData!lowerYear = sortByYear.Last()
            End If


            Filter(make, model, minYear, maxYear, category, minPrice, maxPrice, brand, colour, material, seller)
            Paginate(page, limit)

            Table = tempTable
            Return MyBase.View(WishlistView.CreateNew(repository.SpareList, ViewData), repository.SpareList)
        End Function

        <HttpGet("{id}/{username}"), Route("{id}/AddWishlist")>
        Public Function AddWishlist(id As String, username As String) As String
            Try
                tempTable = Table
                Table = TableContext.WISHLIST
                create = context.Create(New SpareParts() With {.ID = id, .Username = username})
                response = If(create, <div class="alert alert-success">Successfully added to your wishlist.</div>.ToString(),
                        <div class="alert alert-danger">An error occurred while adding this listing to your wishlist.</div>.ToString())
            Catch e As MySql.Data.MySqlClient.MySqlException
                If e.Number = 1062 Then
                    response = <div class="alert alert-danger">This listing already exists in your wishlist.</div>.ToString()
                Else Throw
                End If
            Finally
                Table = tempTable
            End Try
            Return response
        End Function

        <HttpPost(), Route("DeleteWishlist")>
        Public Function DeleteWishlist(id As String, username As String) As IActionResult
            tempTable = Table
            Table = TableContext.WISHLIST
            deleted = context.Delete(New SpareParts() With {.ID = id, .Username = username}) > 0
            TempData!Delete = If(deleted, <div class="alert alert-success">Successfully removed from your wishlist.</div>.ToString(),
                <div class="alert alert-danger">An error occurred while removing this item from your wishlist.</div>.ToString())
            Table = tempTable
            Return MyBase.Redirect(Request.Headers!Referer.ToString())
        End Function


        <HttpGet("{page}/{make?}/{model?}/{minYear?}/{maxYear?}/{category?}/{minPrice?}/{maxPrice?}/{brand?}/{colour?}/{material?}/{seller?}/{limit?}"), Route("Orders")>
        Public Function Orders(page As Int, Optional make As String() = Nothing,
                               Optional model As String() = Nothing, Optional minYear As String = Nothing,
                               Optional maxYear As String = Nothing, Optional category As String() = Nothing,
                               Optional minPrice As String = Nothing, Optional maxPrice As String = Nothing,
                               Optional brand As String() = Nothing, Optional colour As String() = Nothing,
                               Optional material As String() = Nothing, Optional seller As String() = Nothing,
                               Optional limit As Int = 5) As IActionResult
            tempTable = Table
            Table = TableContext.ORDERS
            repository.GetAllListings()
            ViewData!original = repository.SpareList
            PreservedListings = repository.SpareList
            ViewData!getStr = String.Join(";"c, HttpContext.Request.Query.Select(Function(q As KeyValuePair(Of String, StringValues)) String.Format("{0}:{1}", q.Key, q.Value)))

            Dim sortByPrice As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.Price)).Select(Function(r As SpareParts) r.Price)
            If sortByPrice.Any() Then
                ViewData!upperPrice = sortByPrice.First()
                ViewData!lowerPrice = sortByPrice.Last()
            End If
            Dim sortByYear As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.YearOfManufacture)).Select(Function(r As SpareParts) r.YearOfManufacture)
            If sortByYear.Any() Then
                ViewData!upperYear = sortByYear.First()
                ViewData!lowerYear = sortByYear.Last()
            End If


            Filter(make, model, minYear, maxYear, category, minPrice, maxPrice, brand, colour, material, seller)
            Paginate(page, limit)
            Table = tempTable
            Return MyBase.View(OrdersView.CreateNew(repository.SpareList, ViewData), repository.SpareList)
        End Function

        <HttpPost(), Route("AddOrder")>
        Public Function AddOrder(id As String, quantity As String, username As String) As IActionResult
            Try
                tempTable = Table
                Table = TableContext.ORDERS
                Console.WriteLine("ID = {0}{1}Order quantity = {2}{3}Username = {4}", id, vbLf, quantity, vbLf, username)
                create = context.Create(New SpareParts() With {
                    .ID = id,
                    .OrderIdentificationNumber = $"O{Date.Now:yyyyMMddHHmmssff}",
                    .OrderQuantity = quantity,
                    .Username = username
                })
                TempData!Response = If(create, <div class="alert alert-success">Successfully added order.</div>.ToString(),
                        <div class="alert alert-danger">An error occurred.</div>.ToString())
            Catch e As MySql.Data.MySqlClient.MySqlException
                If e.Number = 1062 Then
                    TempData!response = <div class="alert alert-danger">This listing already exists in your orders.</div>.ToString()
                Else Throw
                End If
            Finally
                Table = tempTable
            End Try
            Return MyBase.Redirect(Request.Headers!Referer.ToString())
        End Function


        <HttpPost(), Route("DeleteOrder")>
        Public Function DeleteOrder(id As String, username As String) As IActionResult
            tempTable = Table
            Table = TableContext.ORDERS
            deleted = context.Delete(New SpareParts() With {.ID = id, .Username = username}) > 0
            TempData!Delete = If(deleted, <div class="alert alert-success">Successfully deleted this order.</div>.ToString(),
                <div class="alert alert-danger">An error occurred while deleting this order.</div>.ToString())
            Table = tempTable
            Return MyBase.Redirect(Request.Headers!Referer.ToString())
        End Function

        <HttpGet("{page}/{make?}/{model?}/{minYear?}/{maxYear?}/{category?}/{minPrice?}/{maxPrice?}/{brand?}/{colour?}/{material?}/{seller?}/{limit?}"), Route("Cart")>
        Public Function Cart(page As Int, Optional make As String() = Nothing, Optional model As String() = Nothing,
                             Optional minYear As String = Nothing, Optional maxYear As String = Nothing,
                             Optional category As String() = Nothing, Optional minPrice As String = Nothing,
                             Optional maxPrice As String = Nothing, Optional brand As String() = Nothing,
                             Optional colour As String() = Nothing, Optional material As String() = Nothing,
                             Optional seller As String() = Nothing, Optional limit As Int = 5) As IActionResult
            tempTable = Table
            Table = TableContext.CART
            repository.GetAllListings()
            ViewData!original = repository.SpareList
            PreservedListings = repository.SpareList
            ViewData!getStr = String.Join(";"c, HttpContext.Request.Query.Select(Function(q As KeyValuePair(Of String, StringValues)) String.Format("{0}:{1}", q.Key, q.Value)))

            Dim sortByPrice As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.Price)).Select(Function(r As SpareParts) r.Price)
            If sortByPrice.Any() Then
                ViewData!upperPrice = sortByPrice.First()
                ViewData!lowerPrice = sortByPrice.Last()
            End If
            Dim sortByYear As IEnumerable(Of String) = repository.SpareList.OrderByDescending(Function(r As SpareParts) Val(r.YearOfManufacture)).Select(Function(r As SpareParts) r.YearOfManufacture)
            If sortByYear.Any() Then
                ViewData!upperYear = sortByYear.First()
                ViewData!lowerYear = sortByYear.Last()
            End If

            Filter(make, model, minYear, maxYear, category, minPrice, maxPrice, brand, colour, material, seller)
            Paginate(page, limit)
            Table = tempTable
            Return MyBase.View(CartView.CreateNew(repository.SpareList, ViewData), repository.SpareList)
        End Function

        <HttpGet("{id}/{username}/{quantity}/{maxQuantity}"), Route("{id}/AddCart")>
        Public Function AddCart(id As String, username As String, quantity As String, maxQuantity As String) As String
            Try
                Console.WriteLine("QUANTITY = {0}{1}MAX QUANTITY = {2}", quantity, vbLf, maxQuantity)
                If Val(quantity) < Val(maxQuantity) Then
                    tempTable = Table
                    Table = TableContext.CART
                    create = context.Create(New SpareParts() With {.ID = id, .Username = username, .Quantity = quantity})
                    response = If(create, <div class="alert alert-success">Successfully added to cart.</div>.ToString(),
                    <div class="alert alert-danger">An error occurred while adding this listing to cart.</div>.ToString())
                    Table = tempTable
                Else
                    response = <div class="alert alert-danger">You have exceeded the maximum number of items of purchase.</div>.ToString()
                End If
            Catch e As MySql.Data.MySqlClient.MySqlException
                Console.Error.WriteLine("e.Number={0}{1}e.Code={2}", e.Number, vbLf, e.Code)
                If e.Number = 1062 Then
                    response = <div class="alert alert-danger">This listing already exists in your cart.</div>.ToString()
                Else Throw
                End If
            End Try
            Return response
        End Function

        <HttpPost(), Route("DeleteCart")>
        Public Function DeleteCart(id As String, username As String) As IActionResult
            tempTable = Table
            Table = TableContext.CART
            Console.WriteLine("DELETE FROM CART WITH ID = " & id)
            deleted = context.Delete(New SpareParts() With {.ID = id, .Username = username}) > 0
            TempData!Delete = If(deleted, <div class="alert alert-success">Successfully deleted listing from your cart.</div>.ToString(),
                <div class="alert alert-danger">An error occurred while deleting this item from your cart.</div>.ToString())
            Table = tempTable
            Return MyBase.Redirect(Request.Headers!Referer.ToString())
        End Function

        <HttpGet(), Route("Checkout")>
        Public Function Checkout() As IActionResult
            tempTable = Table
            Table = TableContext.ORDERS
            repository.GetAllListings()
            Table = tempTable

            Return MyBase.View(CheckoutView.CreateNew(repository.SpareList, ViewData))
        End Function

        <ResponseCache(Duration:=0, Location:=ResponseCacheLocation.None, NoStore:=True), HttpGet(), Route("Error")>
        Public Function [Error]() As IActionResult
            Return View(New ErrorViewModel() With {.RequestId = If(Activity.Current?.Id, HttpContext.TraceIdentifier)})
        End Function
    End Class
End Namespace