namespace ProgressusWebApi.Dtos.MercadoPagoDtos
{
	public class PaymentDto
	{
		public object accounts_info { get; set; }
		public List<object> acquirer_reconciliation { get; set; }
		public AdditionalInfo additional_info { get; set; }
		public object authorization_code { get; set; }
		public bool binary_mode { get; set; }
		public object brand_id { get; set; }
		public string build_version { get; set; }
		public object call_for_authorize_id { get; set; }
		public bool captured { get; set; }
		public object card { get; set; }
		public List<ChargesDetail> charges_details { get; set; }
		public ChargesExecutionInfo charges_execution_info { get; set; }
		public long collector_id { get; set; }
		public object corporation_id { get; set; }
		public object counter_currency { get; set; }
		public decimal coupon_amount { get; set; }
		public string currency_id { get; set; }
		public DateTime date_approved { get; set; }
		public DateTime date_created { get; set; }
		public DateTime date_last_updated { get; set; }
		public object date_of_expiration { get; set; }
		public object deduction_schema { get; set; }
		public string description { get; set; }
		public object differential_pricing_id { get; set; }
		public object external_reference { get; set; }
		public List<FeeDetail> fee_details { get; set; }
		public object financing_group { get; set; }
		public long id { get; set; }
		public int installments { get; set; }
		public object integrator_id { get; set; }
		public string issuer_id { get; set; }
		public bool live_mode { get; set; }
		public object marketplace_owner { get; set; }
		public object merchant_account_id { get; set; }
		public object merchant_number { get; set; }
		public Dictionary<string, object> metadata { get; set; }
		public DateTime money_release_date { get; set; }
		public object money_release_schema { get; set; }
		public string money_release_status { get; set; }
		public string notification_url { get; set; }
		public string operation_type { get; set; }
		public Order order { get; set; }
		public Payer payer { get; set; }
		public PaymentMethod payment_method { get; set; }
		public string payment_method_id { get; set; }
		public string payment_type_id { get; set; }
		public object platform_id { get; set; }
		public PointOfInteraction point_of_interaction { get; set; }
		public object pos_id { get; set; }
		public string processing_mode { get; set; }
		public List<object> refunds { get; set; }
		public object release_info { get; set; }
		public decimal shipping_amount { get; set; }
		public object sponsor_id { get; set; }
		public object statement_descriptor { get; set; }
		public string status { get; set; }
		public string status_detail { get; set; }
		public object store_id { get; set; }
		public object tags { get; set; }
		public decimal taxes_amount { get; set; }
		public decimal transaction_amount { get; set; }
		public decimal transaction_amount_refunded { get; set; }
		public TransactionDetails transaction_details { get; set; }
	}
	public class AdditionalInfo
	{
		public object authentication_code { get; set; }
		public object available_balance { get; set; }
		public string ip_address { get; set; }
		public List<Item> items { get; set; }
		public object nsu_processadora { get; set; }
		public Payer payer { get; set; }
	}
	public class Item
	{
		public object category_id { get; set; }
		public string description { get; set; }
		public object id { get; set; }
		public string picture_url { get; set; }
		public string quantity { get; set; }
		public string title { get; set; }
		public int unit_price { get; set; }
	}
	public class Payer
	{
		public string email { get; set; }
		public object entity_type { get; set; }
		public string first_name { get; set; }
		public string id { get; set; }
		public Identification identification { get; set; }
		public string last_name { get; set; }
		public object operator_id { get; set; }
		public Phone phone { get; set; }
		public object type { get; set; }
	}
	public class Identification
	{
		public string number { get; set; }
		public string type { get; set; }
	}
	public class Phone
	{
		public object number { get; set; }
		public object extension { get; set; }
		public object area_code { get; set; }
	}
	public class ChargesDetail
	{
		public Accounts accounts { get; set; }
		public Amounts amounts { get; set; }
		public int client_id { get; set; }
		public DateTime date_created { get; set; }
		public string id { get; set; }
		public DateTime last_updated { get; set; }
		public Metadata metadata { get; set; }
		public string name { get; set; }
		public List<object> refund_charges { get; set; }
		public object reserve_id { get; set; }
		public string type { get; set; }
	}
	public class Accounts
	{
		public string from { get; set; }
		public string to { get; set; }
	}
	public class Amounts
	{
		public decimal original { get; set; }
		public decimal refunded { get; set; }
	}
	public class Metadata
	{
		public string source { get; set; }
	}
	public class ChargesExecutionInfo
	{
		public InternalExecution internal_execution { get; set; }
	}
	public class InternalExecution
	{
		public DateTime date { get; set; }
		public string execution_id { get; set; }
	}
	public class FeeDetail
	{
		public decimal amount { get; set; }
		public string fee_payer { get; set; }
		public string type { get; set; }
	}
	public class Order
	{
		public string id { get; set; }
		public string type { get; set; }
	}
	public class PaymentMethod
	{
		public string id { get; set; }
		public string issuer_id { get; set; }
		public string type { get; set; }
	}
	public class PointOfInteraction
	{
		public BusinessInfo business_info { get; set; }
		public TransactionData transaction_data { get; set; }
		public string type { get; set; }
	}
	public class BusinessInfo
	{
		public string branch { get; set; }
		public string sub_unit { get; set; }
		public string unit { get; set; }
	}
	public class TransactionData
	{
		public object e2e_id { get; set; }
	}
	public class TransactionDetails
	{
		public object acquirer_reference { get; set; }
		public object external_resource_url { get; set; }
		public object financial_institution { get; set; }
		public decimal installment_amount { get; set; }
		public decimal net_received_amount { get; set; }
		public decimal overpaid_amount { get; set; }
		public object payable_deferral_period { get; set; }
		public object payment_method_reference_id { get; set; }
		public decimal total_paid_amount { get; set; }
	}
}
