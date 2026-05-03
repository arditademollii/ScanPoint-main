import React from "react";

interface InvoiceItem {
  name: string;
  price: number;
  quantity: number;
  subTotal: number;
}

interface Invoice {
  id: string;
  serialNumber: string;
  createdAt: string;
  totalAmount: number;
  shop: {
    name: string;
    address: string;
    vatNumber: string;
    fiscalNumber: string;
  };
  cashier: {
    username: string;
  };
  items: InvoiceItem[];
}

interface Props {
  invoice: Invoice;
}

const InvoiceCard: React.FC<Props> = ({ invoice }) => {
  return (
    <div className="border p-4 rounded shadow bg-white">
      <div className="flex justify-between mb-2">
        <div>
          <div className="font-bold text-lg">Invoice: {invoice.serialNumber}</div>
          <div className="text-sm text-gray-500">
            {new Date(invoice.createdAt).toLocaleString()}
          </div>
        </div>
        <div className="text-right">
          <div className="font-bold">{invoice.shop.name}</div>
          <div className="text-sm">{invoice.shop.address}</div>
          <div className="text-sm">VAT: {invoice.shop.vatNumber}</div>
          <div className="text-sm">Fiscal: {invoice.shop.fiscalNumber}</div>
        </div>
      </div>

      <div className="mb-2">
        <span className="font-semibold">Cashier:</span> {invoice.cashier.username}
      </div>

      <table className="w-full border-collapse mb-2">
        <thead>
          <tr className="border-b">
            <th className="text-left p-1">Product</th>
            <th className="text-right p-1">Price</th>
            <th className="text-right p-1">Qty</th>
            <th className="text-right p-1">Subtotal</th>
          </tr>
        </thead>
        <tbody>
          {invoice.items.map((item, idx) => (
            <tr key={idx} className="border-b">
              <td className="p-1">{item.name}</td>
              <td className="p-1 text-right">${item.price.toFixed(2)}</td>
              <td className="p-1 text-right">{item.quantity}</td>
              <td className="p-1 text-right">${item.subTotal.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="text-right font-bold">Total: ${invoice.totalAmount.toFixed(2)}</div>
    </div>
  );
};

export default InvoiceCard;
