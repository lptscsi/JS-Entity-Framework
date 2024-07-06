/*
  Generated from: /RIAppDemoServiceEF/typescript on 2024-07-06 at 13:36
  Don't make manual changes here, they will be lost when this interface will be regenerated!
*/

import * as RIAPP from "jriapp-lib";


//******BEGIN INTERFACE REGION******
export interface IAddressInfo2 {
  AddressId: number;
  AddressLine1: string;
  City: string;
  StateProvince: string;
  CountryRegion: string;
}

/*
  Generated from C# KeyVal model
*/
export interface IKeyVal {
  key: number;
  val: string;
}

export interface IDEMOCLS {
  prodCategory: IKeyVal[];
  prodDescription: IKeyVal[];
  prodModel: IKeyVal[];
}

export interface ITestLookUpProduct {
  ProductId: number;
  Name: string;
}

export enum TestEnum {
  None = 0,
  OK = 1,
  Error = 2,
  Loading = 3
}

/*
  A Class for testing of conversion C# types to typescript
*/
export interface IClientTestModel {
  Key: string;
  SomeProperty1: string;
  SomeProperty2: number[];
  SomeProperty3: string[];
  MoreComplexProperty: ITestLookUpProduct[];
  EnumProperty: TestEnum;
}

/*
  Generated from C# StrKeyVal model
*/
export interface IStrKeyVal {
  key: string;
  val: string;
}

export interface IRadioVal {
  key: string;
  value: string;
  comment: string;
}

/*
  Generated from C# HistoryItem model
*/
export interface IHistoryItem {
  radioValue: string;
  time: Date;
}

/*
  An enum for testing of conversion C# types to typescript
*/
export enum TestEnum2 {
  None = 0,
  One = 1,
  Two = 2,
  Three = 3
}
//******END INTERFACE REGION******

export interface ISvcMethods {
  GetClassifiers: () => RIAPP.IPromise<IDEMOCLS>;
  TestComplexInvoke: (args: {
    info: IAddressInfo2;
    keys: IKeyVal[];
  }) => RIAPP.IPromise<number[]>;
  TestInvoke: (args: {
    param1: number[];
    param2: string;
  }) => RIAPP.IPromise<string>;
}

//******BEGIN LISTS REGION******
export class TestModelListItem extends RIAPP.ListItem {
  get Key(): string { return <string>this._aspect._getProp('Key'); }
  set Key(v: string) { this._aspect._setProp('Key', v); }
  get SomeProperty1(): string { return <string>this._aspect._getProp('SomeProperty1'); }
  set SomeProperty1(v: string) { this._aspect._setProp('SomeProperty1', v); }
  get SomeProperty2(): number[] { return <number[]>this._aspect._getProp('SomeProperty2'); }
  set SomeProperty2(v: number[]) { this._aspect._setProp('SomeProperty2', v); }
  get SomeProperty3(): string[] { return <string[]>this._aspect._getProp('SomeProperty3'); }
  set SomeProperty3(v: string[]) { this._aspect._setProp('SomeProperty3', v); }
  get MoreComplexProperty(): ITestLookUpProduct[] { return <ITestLookUpProduct[]>this._aspect._getProp('MoreComplexProperty'); }
  set MoreComplexProperty(v: ITestLookUpProduct[]) { this._aspect._setProp('MoreComplexProperty', v); }
  get EnumProperty(): TestEnum { return <TestEnum>this._aspect._getProp('EnumProperty'); }
  set EnumProperty(v: TestEnum) { this._aspect._setProp('EnumProperty', v); }
  override toString() {
    return 'TestModelListItem';
  }
}

export class TestDictionary extends RIAPP.BaseDictionary<TestModelListItem> {
  constructor() {
    super('Key', [{ name: 'Key', dtype: 'String' }, { name: 'SomeProperty1', dtype: 'String' }, { name: 'SomeProperty2', dtype: 'Binary' }, { name: 'SomeProperty3', dtype: 'None' }, { name: 'MoreComplexProperty', dtype: 'None' }, { name: 'EnumProperty', dtype: 'None' }]);
  }
  override itemFactory(aspect: RIAPP.ListItemAspect): TestModelListItem {
    return new TestModelListItem(aspect);
  }
  findItem(key: string): TestModelListItem {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString() {
    return 'TestDictionary';
  }
}

export class TestList extends RIAPP.BaseList<TestModelListItem> {
  constructor() {
    super([{ name: 'Key', dtype: 'String' }, { name: 'SomeProperty1', dtype: 'String' }, { name: 'SomeProperty2', dtype: 'Binary' }, { name: 'SomeProperty3', dtype: 'None' }, { name: 'MoreComplexProperty', dtype: 'None' }, { name: 'EnumProperty', dtype: 'None' }]);
  }
  override itemFactory(aspect: RIAPP.ListItemAspect): TestModelListItem {
    return new TestModelListItem(aspect);
  }
  override toString() {
    return 'TestList';
  }
}

export class KeyValListItem extends RIAPP.ListItem {
  get key(): number { return <number>this._aspect._getProp('key'); }
  set key(v: number) { this._aspect._setProp('key', v); }
  get val(): string { return <string>this._aspect._getProp('val'); }
  set val(v: string) { this._aspect._setProp('val', v); }
  override toString() {
    return 'KeyValListItem';
  }
}

export class KeyValDictionary extends RIAPP.BaseDictionary<KeyValListItem> {
  constructor() {
    super('key', [{ name: 'key', dtype: 'Integer' }, { name: 'val', dtype: 'String' }]);
  }
  override itemFactory(aspect: RIAPP.ListItemAspect): KeyValListItem {
    return new KeyValListItem(aspect);
  }
  findItem(key: number): KeyValListItem {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString() {
    return 'KeyValDictionary';
  }
}

export class StrKeyValListItem extends RIAPP.ListItem {
  get key(): string { return <string>this._aspect._getProp('key'); }
  set key(v: string) { this._aspect._setProp('key', v); }
  get val(): string { return <string>this._aspect._getProp('val'); }
  set val(v: string) { this._aspect._setProp('val', v); }
  override toString() {
    return 'StrKeyValListItem';
  }
}

export class StrKeyValDictionary extends RIAPP.BaseDictionary<StrKeyValListItem> {
  constructor() {
    super('key', [{ name: 'key', dtype: 'String' }, { name: 'val', dtype: 'String' }]);
  }
  override itemFactory(aspect: RIAPP.ListItemAspect): StrKeyValListItem {
    return new StrKeyValListItem(aspect);
  }
  findItem(key: string): StrKeyValListItem {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString() {
    return 'StrKeyValDictionary';
  }
}

export class RadioValListItem extends RIAPP.ListItem {
  get key(): string { return <string>this._aspect._getProp('key'); }
  set key(v: string) { this._aspect._setProp('key', v); }
  get value(): string { return <string>this._aspect._getProp('value'); }
  set value(v: string) { this._aspect._setProp('value', v); }
  get comment(): string { return <string>this._aspect._getProp('comment'); }
  set comment(v: string) { this._aspect._setProp('comment', v); }
  override toString() {
    return 'RadioValListItem';
  }
}

export class RadioValDictionary extends RIAPP.BaseDictionary<RadioValListItem> {
  constructor() {
    super('key', [{ name: 'key', dtype: 'String' }, { name: 'value', dtype: 'String' }, { name: 'comment', dtype: 'String' }]);
  }
  override itemFactory(aspect: RIAPP.ListItemAspect): RadioValListItem {
    return new RadioValListItem(aspect);
  }
  findItem(key: string): RadioValListItem {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString() {
    return 'RadioValDictionary';
  }
}

export class HistoryItemListItem extends RIAPP.ListItem {
  get radioValue(): string { return <string>this._aspect._getProp('radioValue'); }
  set radioValue(v: string) { this._aspect._setProp('radioValue', v); }
  get time(): Date { return <Date>this._aspect._getProp('time'); }
  set time(v: Date) { this._aspect._setProp('time', v); }
  override toString() {
    return 'HistoryItemListItem';
  }
}

export class HistoryList extends RIAPP.BaseList<HistoryItemListItem> {
  constructor() {
    super([{ name: 'radioValue', dtype: 'String' }, { name: 'time', dtype: 'DateTime' }]);
  }
  override itemFactory(aspect: RIAPP.ListItemAspect): HistoryItemListItem {
    return new HistoryItemListItem(aspect);
  }
  override toString() {
    return 'HistoryList';
  }
}
//******END LISTS REGION******

//******BEGIN COMPLEX TYPES REGION*****
export class Customer_Contact1 extends RIAPP.ChildComplexProperty {
  constructor(name: string, parent: RIAPP.BaseComplexProperty) {
    super(name, parent);
  }
  get EmailAddress(): string { return this.getValue('CustomerName.Contact.EmailAddress'); }
  set EmailAddress(v: string) { this.setValue('CustomerName.Contact.EmailAddress', v); }
  get Phone(): string { return this.getValue('CustomerName.Contact.Phone'); }
  set Phone(v: string) { this.setValue('CustomerName.Contact.Phone', v); }
  override toString() {
    return 'Customer_Contact1';
  }
}

export class Customer_CustomerName extends RIAPP.RootComplexProperty {
  private _Contact: Customer_Contact1;
  constructor(name: string, owner: RIAPP.EntityAspect) {
    super(name, owner);
    this._Contact = null;
  }
  get Contact(): Customer_Contact1 { if (!this._Contact) { this._Contact = new Customer_Contact1('Contact', this); } return this._Contact; }
  get FirstName(): string { return this.getValue('CustomerName.FirstName'); }
  set FirstName(v: string) { this.setValue('CustomerName.FirstName', v); }
  get LastName(): string { return this.getValue('CustomerName.LastName'); }
  set LastName(v: string) { this.setValue('CustomerName.LastName', v); }
  get MiddleName(): string { return this.getValue('CustomerName.MiddleName'); }
  set MiddleName(v: string) { this.setValue('CustomerName.MiddleName', v); }
  get Name(): string { return this.getEntity()._getCalcFieldVal('CustomerName.Name'); }
  override toString() {
    return 'Customer_CustomerName';
  }
}
//******END COMPLEX TYPES REGION******

export interface IAddress {
  readonly AddressId: number;
  AddressLine1: string;
  AddressLine2: string | null;
  City: string;
  CountryRegion: string;
  ModifiedDate: Date;
  PostalCode: string;
  Rowguid: string;
  StateProvince: string;
}

export class Address extends RIAPP.Entity {
  get AddressId(): number { return this._aspect._getFieldVal('AddressId'); }
  get AddressLine1(): string { return this._aspect._getFieldVal('AddressLine1'); }
  set AddressLine1(v: string) { this._aspect._setFieldVal('AddressLine1', v); }
  get AddressLine2(): string | null { return this._aspect._getFieldVal('AddressLine2'); }
  set AddressLine2(v: string | null) { this._aspect._setFieldVal('AddressLine2', v); }
  get City(): string { return this._aspect._getFieldVal('City'); }
  set City(v: string) { this._aspect._setFieldVal('City', v); }
  get CountryRegion(): string { return this._aspect._getFieldVal('CountryRegion'); }
  set CountryRegion(v: string) { this._aspect._setFieldVal('CountryRegion', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  set ModifiedDate(v: Date) { this._aspect._setFieldVal('ModifiedDate', v); }
  get PostalCode(): string { return this._aspect._getFieldVal('PostalCode'); }
  set PostalCode(v: string) { this._aspect._setFieldVal('PostalCode', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  set Rowguid(v: string) { this._aspect._setFieldVal('Rowguid', v); }
  get StateProvince(): string { return this._aspect._getFieldVal('StateProvince'); }
  set StateProvince(v: string) { this._aspect._setFieldVal('StateProvince', v); }
  get CustomerAddress(): CustomerAddress[] { return this._aspect._getNavFieldVal('CustomerAddress'); }
  get SalesOrderHeaderBillToAddress(): SalesOrderHeader[] { return this._aspect._getNavFieldVal('SalesOrderHeaderBillToAddress'); }
  get SalesOrderHeaderShipToAddress(): SalesOrderHeader[] { return this._aspect._getNavFieldVal('SalesOrderHeaderShipToAddress'); }
  override toString() {
    return 'Address';
  }
}

export class AddressDb extends RIAPP.DbSet<Address>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new Address(aspect);
    super(opts);
  }
  findEntity(addressId: number): Address {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'AddressDb';
  }
  createReadAddressByIdsQuery(args?: {
    addressIDs: number[];
  }): RIAPP.DataQuery<Address> {
    var query = this.createQuery('ReadAddressByIds');
    query.params = args;
    return query;
  }
  createReadAddressQuery(): RIAPP.DataQuery<Address> {
    return this.createQuery('ReadAddress');
  }
}

export interface IAddressInfo {
  readonly AddressId: number;
  readonly AddressLine1: string | null;
  readonly City: string | null;
  readonly StateProvince: string | null;
  readonly CountryRegion: string | null;
}

export class AddressInfo extends RIAPP.Entity {
  get AddressId(): number { return this._aspect._getFieldVal('AddressId'); }
  get AddressLine1(): string | null { return this._aspect._getFieldVal('AddressLine1'); }
  get City(): string | null { return this._aspect._getFieldVal('City'); }
  get StateProvince(): string | null { return this._aspect._getFieldVal('StateProvince'); }
  get CountryRegion(): string | null { return this._aspect._getFieldVal('CountryRegion'); }
  get CustomerAddresses(): CustomerAddress[] { return this._aspect._getNavFieldVal('CustomerAddresses'); }
  override toString() {
    return 'AddressInfo';
  }
}

export class AddressInfoDb extends RIAPP.DbSet<AddressInfo>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new AddressInfo(aspect);
    super(opts);
  }
  findEntity(addressId: number): AddressInfo {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'AddressInfoDb';
  }
  createReadAddressInfoQuery(): RIAPP.DataQuery<AddressInfo> {
    return this.createQuery('ReadAddressInfo');
  }
}

export interface ICustomer {
  readonly CustomerName: Customer_CustomerName;
  readonly CustomerId: number;
  CompanyName: string | null;
  readonly ModifiedDate: Date;
  NameStyle: boolean;
  PasswordHash: string;
  PasswordSalt: string;
  readonly Rowguid: string;
  SalesPerson: string | null;
  Suffix: string | null;
  Title: string | null;
  AddressCount: number | null;
}

export class Customer extends RIAPP.Entity {
  private _CustomerName: Customer_CustomerName;
  get CustomerName(): Customer_CustomerName { if (!this._CustomerName) { this._CustomerName = new Customer_CustomerName('CustomerName', this._aspect); } return this._CustomerName; }
  get CustomerId(): number { return this._aspect._getFieldVal('CustomerId'); }
  get CompanyName(): string | null { return this._aspect._getFieldVal('CompanyName'); }
  set CompanyName(v: string | null) { this._aspect._setFieldVal('CompanyName', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  get NameStyle(): boolean { return this._aspect._getFieldVal('NameStyle'); }
  set NameStyle(v: boolean) { this._aspect._setFieldVal('NameStyle', v); }
  get PasswordHash(): string { return this._aspect._getFieldVal('PasswordHash'); }
  set PasswordHash(v: string) { this._aspect._setFieldVal('PasswordHash', v); }
  get PasswordSalt(): string { return this._aspect._getFieldVal('PasswordSalt'); }
  set PasswordSalt(v: string) { this._aspect._setFieldVal('PasswordSalt', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  get SalesPerson(): string | null { return this._aspect._getFieldVal('SalesPerson'); }
  set SalesPerson(v: string | null) { this._aspect._setFieldVal('SalesPerson', v); }
  get Suffix(): string | null { return this._aspect._getFieldVal('Suffix'); }
  set Suffix(v: string | null) { this._aspect._setFieldVal('Suffix', v); }
  get Title(): string | null { return this._aspect._getFieldVal('Title'); }
  set Title(v: string | null) { this._aspect._setFieldVal('Title', v); }
  get AddressCount(): number | null { return this._aspect._getFieldVal('AddressCount'); }
  set AddressCount(v: number | null) { this._aspect._setFieldVal('AddressCount', v); }
  get CustomerAddress(): CustomerAddress[] { return this._aspect._getNavFieldVal('CustomerAddress'); }
  get SalesOrderHeader(): SalesOrderHeader[] { return this._aspect._getNavFieldVal('SalesOrderHeader'); }
  override toString() {
    return 'Customer';
  }
}

export class CustomerDb extends RIAPP.DbSet<Customer>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new Customer(aspect);
    super(opts);
  }
  findEntity(customerId: number): Customer {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'CustomerDb';
  }
  createReadCustomerQuery(args?: {
    includeNav?: boolean;
  }): RIAPP.DataQuery<Customer> {
    var query = this.createQuery('ReadCustomer');
    query.params = args;
    return query;
  }
  defineCustomerName_NameField(getFunc: (item: Customer) => string | null) { this.defineCalculatedField('CustomerName.Name', getFunc); }
}

export interface ICustomerAddress {
  CustomerId: number;
  AddressId: number;
  AddressType: string;
  ModifiedDate: Date;
  Rowguid: string;
}

export class CustomerAddress extends RIAPP.Entity {
  get CustomerId(): number { return this._aspect._getFieldVal('CustomerId'); }
  set CustomerId(v: number) { this._aspect._setFieldVal('CustomerId', v); }
  get AddressId(): number { return this._aspect._getFieldVal('AddressId'); }
  set AddressId(v: number) { this._aspect._setFieldVal('AddressId', v); }
  get AddressType(): string { return this._aspect._getFieldVal('AddressType'); }
  set AddressType(v: string) { this._aspect._setFieldVal('AddressType', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  set ModifiedDate(v: Date) { this._aspect._setFieldVal('ModifiedDate', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  set Rowguid(v: string) { this._aspect._setFieldVal('Rowguid', v); }
  get Address(): Address { return this._aspect._getNavFieldVal('Address'); }
  set Address(v: Address) { this._aspect._setNavFieldVal('Address', v); }
  get AddressInfo(): AddressInfo { return this._aspect._getNavFieldVal('AddressInfo'); }
  set AddressInfo(v: AddressInfo) { this._aspect._setNavFieldVal('AddressInfo', v); }
  get Customer(): Customer { return this._aspect._getNavFieldVal('Customer'); }
  set Customer(v: Customer) { this._aspect._setNavFieldVal('Customer', v); }
  override toString() {
    return 'CustomerAddress';
  }
}

export class CustomerAddressDb extends RIAPP.DbSet<CustomerAddress>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new CustomerAddress(aspect);
    super(opts);
  }
  findEntity(customerId: number, addressId: number): CustomerAddress {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'CustomerAddressDb';
  }
  createReadAddressForCustomersQuery(args?: {
    custIDs: number[];
  }): RIAPP.DataQuery<CustomerAddress> {
    var query = this.createQuery('ReadAddressForCustomers');
    query.params = args;
    return query;
  }
  createReadCustomerAddressQuery(): RIAPP.DataQuery<CustomerAddress> {
    return this.createQuery('ReadCustomerAddress');
  }
}

export interface ICustomerJSON {
  readonly CustomerId: number;
  Data: string;
  readonly Rowguid: string;
}

export class CustomerJSON extends RIAPP.Entity {
  get CustomerId(): number { return this._aspect._getFieldVal('CustomerId'); }
  get Data(): string { return this._aspect._getFieldVal('Data'); }
  set Data(v: string) { this._aspect._setFieldVal('Data', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  get Customer(): any | null { return this._aspect._getCalcFieldVal('Customer'); }
  override toString() {
    return 'CustomerJSON';
  }
}

export class CustomerJSONDb extends RIAPP.DbSet<CustomerJSON>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new CustomerJSON(aspect);
    super(opts);
  }
  findEntity(customerId: number): CustomerJSON {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'CustomerJSONDb';
  }
  createReadCustomerJSONQuery(): RIAPP.DataQuery<CustomerJSON> {
    return this.createQuery('ReadCustomerJSON');
  }
  defineCustomerField(getFunc: (item: CustomerJSON) => any | null) { this.defineCalculatedField('Customer', getFunc); }
}

export interface ILookUpProduct {
  ProductId: number;
  Name: string;
}

export class LookUpProduct extends RIAPP.Entity {
  get ProductId(): number { return this._aspect._getFieldVal('ProductId'); }
  set ProductId(v: number) { this._aspect._setFieldVal('ProductId', v); }
  get Name(): string { return this._aspect._getFieldVal('Name'); }
  set Name(v: string) { this._aspect._setFieldVal('Name', v); }
  override toString() {
    return 'LookUpProduct';
  }
}

export class LookUpProductDb extends RIAPP.DbSet<LookUpProduct>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new LookUpProduct(aspect);
    super(opts);
  }
  findEntity(productId: number): LookUpProduct {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'LookUpProductDb';
  }
  createReadProductLookUpQuery(): RIAPP.DataQuery<LookUpProduct> {
    return this.createQuery('ReadProductLookUp');
  }
}

export interface IProduct {
  readonly ProductId: number;
  Color: string | null;
  DiscontinuedDate: Date | null;
  ListPrice: number;
  readonly ModifiedDate: Date;
  Name: string;
  ProductCategoryId: number | null;
  ProductModelId: number | null;
  ProductNumber: string;
  readonly Rowguid: string;
  SellEndDate: Date | null;
  SellStartDate: Date;
  Size: string | null;
  StandardCost: number;
  readonly ThumbnailPhotoFileName: string | null;
  Weight: number | null;
}

export class Product extends RIAPP.Entity {
  get ProductId(): number { return this._aspect._getFieldVal('ProductId'); }
  get Color(): string | null { return this._aspect._getFieldVal('Color'); }
  set Color(v: string | null) { this._aspect._setFieldVal('Color', v); }
  get DiscontinuedDate(): Date | null { return this._aspect._getFieldVal('DiscontinuedDate'); }
  set DiscontinuedDate(v: Date | null) { this._aspect._setFieldVal('DiscontinuedDate', v); }
  get ListPrice(): number { return this._aspect._getFieldVal('ListPrice'); }
  set ListPrice(v: number) { this._aspect._setFieldVal('ListPrice', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  get Name(): string { return this._aspect._getFieldVal('Name'); }
  set Name(v: string) { this._aspect._setFieldVal('Name', v); }
  get ProductCategoryId(): number | null { return this._aspect._getFieldVal('ProductCategoryId'); }
  set ProductCategoryId(v: number | null) { this._aspect._setFieldVal('ProductCategoryId', v); }
  get ProductModelId(): number | null { return this._aspect._getFieldVal('ProductModelId'); }
  set ProductModelId(v: number | null) { this._aspect._setFieldVal('ProductModelId', v); }
  get ProductNumber(): string { return this._aspect._getFieldVal('ProductNumber'); }
  set ProductNumber(v: string) { this._aspect._setFieldVal('ProductNumber', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  get SellEndDate(): Date | null { return this._aspect._getFieldVal('SellEndDate'); }
  set SellEndDate(v: Date | null) { this._aspect._setFieldVal('SellEndDate', v); }
  get SellStartDate(): Date { return this._aspect._getFieldVal('SellStartDate'); }
  set SellStartDate(v: Date) { this._aspect._setFieldVal('SellStartDate', v); }
  get Size(): string | null { return this._aspect._getFieldVal('Size'); }
  set Size(v: string | null) { this._aspect._setFieldVal('Size', v); }
  get StandardCost(): number { return this._aspect._getFieldVal('StandardCost'); }
  set StandardCost(v: number) { this._aspect._setFieldVal('StandardCost', v); }
  get ThumbnailPhotoFileName(): string | null { return this._aspect._getFieldVal('ThumbnailPhotoFileName'); }
  get Weight(): number | null { return this._aspect._getFieldVal('Weight'); }
  set Weight(v: number | null) { this._aspect._setFieldVal('Weight', v); }
  get IsActive(): boolean | null { return this._aspect._getCalcFieldVal('IsActive'); }
  get ProductCategory(): ProductCategory { return this._aspect._getNavFieldVal('ProductCategory'); }
  set ProductCategory(v: ProductCategory) { this._aspect._setNavFieldVal('ProductCategory', v); }
  get ProductModel(): ProductModel { return this._aspect._getNavFieldVal('ProductModel'); }
  set ProductModel(v: ProductModel) { this._aspect._setNavFieldVal('ProductModel', v); }
  get SalesOrderDetail(): SalesOrderDetail[] { return this._aspect._getNavFieldVal('SalesOrderDetail'); }
  override toString() {
    return 'Product';
  }
}

export class ProductDb extends RIAPP.DbSet<Product>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new Product(aspect);
    super(opts);
  }
  findEntity(productId: number): Product {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'ProductDb';
  }
  createReadProductByIdsQuery(args?: {
    productIDs: number[];
  }): RIAPP.DataQuery<Product> {
    var query = this.createQuery('ReadProductByIds');
    query.params = args;
    return query;
  }
  createReadProductQuery(args?: {
    param1: number[];
    param2: string;
  }): RIAPP.DataQuery<Product> {
    var query = this.createQuery('ReadProduct');
    query.params = args;
    return query;
  }
  defineIsActiveField(getFunc: (item: Product) => boolean | null) { this.defineCalculatedField('IsActive', getFunc); }
}

export interface IProductCategory {
  readonly ProductCategoryId: number;
  ModifiedDate: Date;
  Name: string;
  ParentProductCategoryId: number | null;
  Rowguid: string;
}

export class ProductCategory extends RIAPP.Entity {
  get ProductCategoryId(): number { return this._aspect._getFieldVal('ProductCategoryId'); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  set ModifiedDate(v: Date) { this._aspect._setFieldVal('ModifiedDate', v); }
  get Name(): string { return this._aspect._getFieldVal('Name'); }
  set Name(v: string) { this._aspect._setFieldVal('Name', v); }
  get ParentProductCategoryId(): number | null { return this._aspect._getFieldVal('ParentProductCategoryId'); }
  set ParentProductCategoryId(v: number | null) { this._aspect._setFieldVal('ParentProductCategoryId', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  set Rowguid(v: string) { this._aspect._setFieldVal('Rowguid', v); }
  get Product(): Product[] { return this._aspect._getNavFieldVal('Product'); }
  get ParentProductCategory(): ProductCategory { return this._aspect._getNavFieldVal('ParentProductCategory'); }
  set ParentProductCategory(v: ProductCategory) { this._aspect._setNavFieldVal('ParentProductCategory', v); }
  get InverseParentProductCategory(): ProductCategory[] { return this._aspect._getNavFieldVal('InverseParentProductCategory'); }
  override toString() {
    return 'ProductCategory';
  }
}

export class ProductCategoryDb extends RIAPP.DbSet<ProductCategory>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new ProductCategory(aspect);
    super(opts);
  }
  findEntity(productCategoryId: number): ProductCategory {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'ProductCategoryDb';
  }
  createReadProductCategoryQuery(): RIAPP.DataQuery<ProductCategory> {
    return this.createQuery('ReadProductCategory');
  }
}

export interface IProductDescription {
  readonly ProductDescriptionId: number;
  Description: string;
  ModifiedDate: Date;
  Rowguid: string;
}

export class ProductDescription extends RIAPP.Entity {
  get ProductDescriptionId(): number { return this._aspect._getFieldVal('ProductDescriptionId'); }
  get Description(): string { return this._aspect._getFieldVal('Description'); }
  set Description(v: string) { this._aspect._setFieldVal('Description', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  set ModifiedDate(v: Date) { this._aspect._setFieldVal('ModifiedDate', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  set Rowguid(v: string) { this._aspect._setFieldVal('Rowguid', v); }
  get ProductModelProductDescription(): ProductModelProductDescription[] { return this._aspect._getNavFieldVal('ProductModelProductDescription'); }
  override toString() {
    return 'ProductDescription';
  }
}

export class ProductDescriptionDb extends RIAPP.DbSet<ProductDescription>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new ProductDescription(aspect);
    super(opts);
  }
  findEntity(productDescriptionId: number): ProductDescription {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'ProductDescriptionDb';
  }
}

export interface IProductModel {
  readonly ProductModelId: number;
  CatalogDescription: string | null;
  ModifiedDate: Date;
  Name: string;
  Rowguid: string;
}

export class ProductModel extends RIAPP.Entity {
  get ProductModelId(): number { return this._aspect._getFieldVal('ProductModelId'); }
  get CatalogDescription(): string | null { return this._aspect._getFieldVal('CatalogDescription'); }
  set CatalogDescription(v: string | null) { this._aspect._setFieldVal('CatalogDescription', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  set ModifiedDate(v: Date) { this._aspect._setFieldVal('ModifiedDate', v); }
  get Name(): string { return this._aspect._getFieldVal('Name'); }
  set Name(v: string) { this._aspect._setFieldVal('Name', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  set Rowguid(v: string) { this._aspect._setFieldVal('Rowguid', v); }
  get Product(): Product[] { return this._aspect._getNavFieldVal('Product'); }
  get ProductModelProductDescription(): ProductModelProductDescription[] { return this._aspect._getNavFieldVal('ProductModelProductDescription'); }
  override toString() {
    return 'ProductModel';
  }
}

export class ProductModelDb extends RIAPP.DbSet<ProductModel>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new ProductModel(aspect);
    super(opts);
  }
  findEntity(productModelId: number): ProductModel {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'ProductModelDb';
  }
  createReadProductModelQuery(): RIAPP.DataQuery<ProductModel> {
    return this.createQuery('ReadProductModel');
  }
}

export interface IProductModelProductDescription {
  ProductModelId: number;
  ProductDescriptionId: number;
  Culture: string;
  ModifiedDate: Date;
  Rowguid: string;
}

export class ProductModelProductDescription extends RIAPP.Entity {
  get ProductModelId(): number { return this._aspect._getFieldVal('ProductModelId'); }
  set ProductModelId(v: number) { this._aspect._setFieldVal('ProductModelId', v); }
  get ProductDescriptionId(): number { return this._aspect._getFieldVal('ProductDescriptionId'); }
  set ProductDescriptionId(v: number) { this._aspect._setFieldVal('ProductDescriptionId', v); }
  get Culture(): string { return this._aspect._getFieldVal('Culture'); }
  set Culture(v: string) { this._aspect._setFieldVal('Culture', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  set ModifiedDate(v: Date) { this._aspect._setFieldVal('ModifiedDate', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  set Rowguid(v: string) { this._aspect._setFieldVal('Rowguid', v); }
  get ProductDescription(): ProductDescription { return this._aspect._getNavFieldVal('ProductDescription'); }
  set ProductDescription(v: ProductDescription) { this._aspect._setNavFieldVal('ProductDescription', v); }
  get ProductModel(): ProductModel { return this._aspect._getNavFieldVal('ProductModel'); }
  set ProductModel(v: ProductModel) { this._aspect._setNavFieldVal('ProductModel', v); }
  override toString() {
    return 'ProductModelProductDescription';
  }
}

export class ProductModelProductDescriptionDb extends RIAPP.DbSet<ProductModelProductDescription>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new ProductModelProductDescription(aspect);
    super(opts);
  }
  findEntity(productModelId: number, productDescriptionId: number, culture: string): ProductModelProductDescription {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'ProductModelProductDescriptionDb';
  }
}

export interface ISalesInfo {
  SalesPerson: string;
}

export class SalesInfo extends RIAPP.Entity {
  get SalesPerson(): string { return this._aspect._getFieldVal('SalesPerson'); }
  set SalesPerson(v: string) { this._aspect._setFieldVal('SalesPerson', v); }
  override toString() {
    return 'SalesInfo';
  }
}

export class SalesInfoDb extends RIAPP.DbSet<SalesInfo>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new SalesInfo(aspect);
    super(opts);
  }
  findEntity(salesPerson: string): SalesInfo {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'SalesInfoDb';
  }
  createReadSalesInfoQuery(): RIAPP.DataQuery<SalesInfo> {
    return this.createQuery('ReadSalesInfo');
  }
}

export interface ISalesOrderDetail {
  SalesOrderId: number;
  readonly SalesOrderDetailId: number;
  LineTotal: number;
  ModifiedDate: Date;
  OrderQty: number;
  ProductId: number;
  Rowguid: string;
  UnitPrice: number;
  UnitPriceDiscount: number;
}

export class SalesOrderDetail extends RIAPP.Entity {
  get SalesOrderId(): number { return this._aspect._getFieldVal('SalesOrderId'); }
  set SalesOrderId(v: number) { this._aspect._setFieldVal('SalesOrderId', v); }
  get SalesOrderDetailId(): number { return this._aspect._getFieldVal('SalesOrderDetailId'); }
  get LineTotal(): number { return this._aspect._getFieldVal('LineTotal'); }
  set LineTotal(v: number) { this._aspect._setFieldVal('LineTotal', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  set ModifiedDate(v: Date) { this._aspect._setFieldVal('ModifiedDate', v); }
  get OrderQty(): number { return this._aspect._getFieldVal('OrderQty'); }
  set OrderQty(v: number) { this._aspect._setFieldVal('OrderQty', v); }
  get ProductId(): number { return this._aspect._getFieldVal('ProductId'); }
  set ProductId(v: number) { this._aspect._setFieldVal('ProductId', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  set Rowguid(v: string) { this._aspect._setFieldVal('Rowguid', v); }
  get UnitPrice(): number { return this._aspect._getFieldVal('UnitPrice'); }
  set UnitPrice(v: number) { this._aspect._setFieldVal('UnitPrice', v); }
  get UnitPriceDiscount(): number { return this._aspect._getFieldVal('UnitPriceDiscount'); }
  set UnitPriceDiscount(v: number) { this._aspect._setFieldVal('UnitPriceDiscount', v); }
  get Product(): Product { return this._aspect._getNavFieldVal('Product'); }
  set Product(v: Product) { this._aspect._setNavFieldVal('Product', v); }
  get SalesOrder(): SalesOrderHeader { return this._aspect._getNavFieldVal('SalesOrder'); }
  set SalesOrder(v: SalesOrderHeader) { this._aspect._setNavFieldVal('SalesOrder', v); }
  override toString() {
    return 'SalesOrderDetail';
  }
}

export class SalesOrderDetailDb extends RIAPP.DbSet<SalesOrderDetail>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new SalesOrderDetail(aspect);
    super(opts);
  }
  findEntity(salesOrderId: number, salesOrderDetailId: number): SalesOrderDetail {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'SalesOrderDetailDb';
  }
  createReadSalesOrderDetailQuery(): RIAPP.DataQuery<SalesOrderDetail> {
    return this.createQuery('ReadSalesOrderDetail');
  }
}

export interface ISalesOrderHeader {
  readonly SalesOrderId: number;
  AccountNumber: string | null;
  BillToAddressId: number | null;
  Comment: string | null;
  CreditCardApprovalCode: string | null;
  CustomerId: number;
  DueDate: Date;
  Freight: number;
  ModifiedDate: Date;
  OnlineOrderFlag: boolean;
  OrderDate: Date;
  PurchaseOrderNumber: string | null;
  RevisionNumber: number;
  Rowguid: string;
  SalesOrderNumber: string;
  ShipDate: Date | null;
  ShipMethod: string;
  ShipToAddressId: number | null;
  Status: number;
  SubTotal: number;
  TaxAmt: number;
  TotalDue: number;
}

export class SalesOrderHeader extends RIAPP.Entity {
  get SalesOrderId(): number { return this._aspect._getFieldVal('SalesOrderId'); }
  get AccountNumber(): string | null { return this._aspect._getFieldVal('AccountNumber'); }
  set AccountNumber(v: string | null) { this._aspect._setFieldVal('AccountNumber', v); }
  get BillToAddressId(): number | null { return this._aspect._getFieldVal('BillToAddressId'); }
  set BillToAddressId(v: number | null) { this._aspect._setFieldVal('BillToAddressId', v); }
  get Comment(): string | null { return this._aspect._getFieldVal('Comment'); }
  set Comment(v: string | null) { this._aspect._setFieldVal('Comment', v); }
  get CreditCardApprovalCode(): string | null { return this._aspect._getFieldVal('CreditCardApprovalCode'); }
  set CreditCardApprovalCode(v: string | null) { this._aspect._setFieldVal('CreditCardApprovalCode', v); }
  get CustomerId(): number { return this._aspect._getFieldVal('CustomerId'); }
  set CustomerId(v: number) { this._aspect._setFieldVal('CustomerId', v); }
  get DueDate(): Date { return this._aspect._getFieldVal('DueDate'); }
  set DueDate(v: Date) { this._aspect._setFieldVal('DueDate', v); }
  get Freight(): number { return this._aspect._getFieldVal('Freight'); }
  set Freight(v: number) { this._aspect._setFieldVal('Freight', v); }
  get ModifiedDate(): Date { return this._aspect._getFieldVal('ModifiedDate'); }
  set ModifiedDate(v: Date) { this._aspect._setFieldVal('ModifiedDate', v); }
  get OnlineOrderFlag(): boolean { return this._aspect._getFieldVal('OnlineOrderFlag'); }
  set OnlineOrderFlag(v: boolean) { this._aspect._setFieldVal('OnlineOrderFlag', v); }
  get OrderDate(): Date { return this._aspect._getFieldVal('OrderDate'); }
  set OrderDate(v: Date) { this._aspect._setFieldVal('OrderDate', v); }
  get PurchaseOrderNumber(): string | null { return this._aspect._getFieldVal('PurchaseOrderNumber'); }
  set PurchaseOrderNumber(v: string | null) { this._aspect._setFieldVal('PurchaseOrderNumber', v); }
  get RevisionNumber(): number { return this._aspect._getFieldVal('RevisionNumber'); }
  set RevisionNumber(v: number) { this._aspect._setFieldVal('RevisionNumber', v); }
  get Rowguid(): string { return this._aspect._getFieldVal('Rowguid'); }
  set Rowguid(v: string) { this._aspect._setFieldVal('Rowguid', v); }
  get SalesOrderNumber(): string { return this._aspect._getFieldVal('SalesOrderNumber'); }
  set SalesOrderNumber(v: string) { this._aspect._setFieldVal('SalesOrderNumber', v); }
  get ShipDate(): Date | null { return this._aspect._getFieldVal('ShipDate'); }
  set ShipDate(v: Date | null) { this._aspect._setFieldVal('ShipDate', v); }
  get ShipMethod(): string { return this._aspect._getFieldVal('ShipMethod'); }
  set ShipMethod(v: string) { this._aspect._setFieldVal('ShipMethod', v); }
  get ShipToAddressId(): number | null { return this._aspect._getFieldVal('ShipToAddressId'); }
  set ShipToAddressId(v: number | null) { this._aspect._setFieldVal('ShipToAddressId', v); }
  get Status(): number { return this._aspect._getFieldVal('Status'); }
  set Status(v: number) { this._aspect._setFieldVal('Status', v); }
  get SubTotal(): number { return this._aspect._getFieldVal('SubTotal'); }
  set SubTotal(v: number) { this._aspect._setFieldVal('SubTotal', v); }
  get TaxAmt(): number { return this._aspect._getFieldVal('TaxAmt'); }
  set TaxAmt(v: number) { this._aspect._setFieldVal('TaxAmt', v); }
  get TotalDue(): number { return this._aspect._getFieldVal('TotalDue'); }
  set TotalDue(v: number) { this._aspect._setFieldVal('TotalDue', v); }
  get SalesOrderDetail(): SalesOrderDetail[] { return this._aspect._getNavFieldVal('SalesOrderDetail'); }
  get BillToAddress(): Address { return this._aspect._getNavFieldVal('BillToAddress'); }
  set BillToAddress(v: Address) { this._aspect._setNavFieldVal('BillToAddress', v); }
  get Customer(): Customer { return this._aspect._getNavFieldVal('Customer'); }
  set Customer(v: Customer) { this._aspect._setNavFieldVal('Customer', v); }
  get ShipToAddress(): Address { return this._aspect._getNavFieldVal('ShipToAddress'); }
  set ShipToAddress(v: Address) { this._aspect._setNavFieldVal('ShipToAddress', v); }
  override toString() {
    return 'SalesOrderHeader';
  }
}

export class SalesOrderHeaderDb extends RIAPP.DbSet<SalesOrderHeader>
{
  constructor(opts: RIAPP.IDbSetConstuctorOptions) {
    opts.itemFactory = (aspect) => new SalesOrderHeader(aspect);
    super(opts);
  }
  findEntity(salesOrderId: number): SalesOrderHeader {
    return this.findByPK(RIAPP.Utils.arr.fromList(arguments));
  }
  override toString(): string {
    return 'SalesOrderHeaderDb';
  }
  createReadSalesOrderHeaderQuery(): RIAPP.DataQuery<SalesOrderHeader> {
    return this.createQuery('ReadSalesOrderHeader');
  }
}

export interface IAssocs {
  getCustAddr_AddressInfo: () => RIAPP.Association;
  getCustomerAddress_Address: () => RIAPP.Association;
  getCustomerAddress_Customer: () => RIAPP.Association;
  getInverseParentProductCategory_ParentProductCategory: () => RIAPP.Association;
  getProduct_ProductCategory: () => RIAPP.Association;
  getProduct_ProductModel: () => RIAPP.Association;
  getProductModelProductDescription_ProductDescription: () => RIAPP.Association;
  getProductModelProductDescription_ProductModel: () => RIAPP.Association;
  getSalesOrderDetail_Product: () => RIAPP.Association;
  getSalesOrderDetail_SalesOrder: () => RIAPP.Association;
  getSalesOrderHeader_Customer: () => RIAPP.Association;
  getSalesOrderHeaderBillToAddress_BillToAddress: () => RIAPP.Association;
  getSalesOrderHeaderShipToAddress_ShipToAddress: () => RIAPP.Association;
}


export class DbSets extends RIAPP.DbSets {
  constructor() {
    super();
    this._createDbSet("Address", (options) => new AddressDb(options));
    this._createDbSet("AddressInfo", (options) => new AddressInfoDb(options));
    this._createDbSet("Customer", (options) => new CustomerDb(options));
    this._createDbSet("CustomerAddress", (options) => new CustomerAddressDb(options));
    this._createDbSet("CustomerJSON", (options) => new CustomerJSONDb(options));
    this._createDbSet("LookUpProduct", (options) => new LookUpProductDb(options));
    this._createDbSet("Product", (options) => new ProductDb(options));
    this._createDbSet("ProductCategory", (options) => new ProductCategoryDb(options));
    this._createDbSet("ProductDescription", (options) => new ProductDescriptionDb(options));
    this._createDbSet("ProductModel", (options) => new ProductModelDb(options));
    this._createDbSet("ProductModelProductDescription", (options) => new ProductModelProductDescriptionDb(options));
    this._createDbSet("SalesInfo", (options) => new SalesInfoDb(options));
    this._createDbSet("SalesOrderDetail", (options) => new SalesOrderDetailDb(options));
    this._createDbSet("SalesOrderHeader", (options) => new SalesOrderHeaderDb(options));
  }
  get Address() { return <AddressDb>this.getDbSet("Address"); }
  get AddressInfo() { return <AddressInfoDb>this.getDbSet("AddressInfo"); }
  get Customer() { return <CustomerDb>this.getDbSet("Customer"); }
  get CustomerAddress() { return <CustomerAddressDb>this.getDbSet("CustomerAddress"); }
  get CustomerJSON() { return <CustomerJSONDb>this.getDbSet("CustomerJSON"); }
  get LookUpProduct() { return <LookUpProductDb>this.getDbSet("LookUpProduct"); }
  get Product() { return <ProductDb>this.getDbSet("Product"); }
  get ProductCategory() { return <ProductCategoryDb>this.getDbSet("ProductCategory"); }
  get ProductDescription() { return <ProductDescriptionDb>this.getDbSet("ProductDescription"); }
  get ProductModel() { return <ProductModelDb>this.getDbSet("ProductModel"); }
  get ProductModelProductDescription() { return <ProductModelProductDescriptionDb>this.getDbSet("ProductModelProductDescription"); }
  get SalesInfo() { return <SalesInfoDb>this.getDbSet("SalesInfo"); }
  get SalesOrderDetail() { return <SalesOrderDetailDb>this.getDbSet("SalesOrderDetail"); }
  get SalesOrderHeader() { return <SalesOrderHeaderDb>this.getDbSet("SalesOrderHeader"); }
}
export class DbContext extends RIAPP.DbContext<ISvcMethods, IAssocs, DbSets>
{
  protected override _provideDbSets(): DbSets {
    return new DbSets();
  }
}
