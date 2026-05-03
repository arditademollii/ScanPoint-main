// src/components/InvoiceList.tsx
import React from "react";

// DTO që REALISHT vjen nga backend
export interface InvoiceListDto {
  id: string;
  serialNumber?: string;
  createdAt: string;
  totalAmount: number;

  shopName?: string;
  cashierName?: string;
}

interface InvoiceListProps {
  invoices: InvoiceListDto[];
}

const InvoiceList: React.FC<InvoiceListProps> = ({ invoices }) => {
  if (!invoices || invoices.length === 0) {
    return <p>No invoices found.</p>;
  }

  return (
    <table className="min-w-full border border-gray-300">
      <thead>
        <tr className="bg-gray-100">
          <th className="px-4 py-2 border">ID</th>
          <th className="px-4 py-2 border">Cashier</th>
          <th className="px-4 py-2 border">Shop</th>
          <th className="px-4 py-2 border">Created At</th>
          <th className="px-4 py-2 border">Total</th>
        </tr>
      </thead>
      <tbody>
        {invoices.map((invoice) => (
          <tr key={invoice.id} className="hover:bg-gray-50">
            <td className="px-4 py-2 border">{invoice.id}</td>

            <td className="px-4 py-2 border">
              {invoice.cashierName || "Unknown"}
            </td>

            <td className="px-4 py-2 border">
              {invoice.shopName || "Unknown"}
            </td>

            <td className="px-4 py-2 border">
              {new Date(invoice.createdAt).toLocaleString()}
            </td>

            <td className="px-4 py-2 border">
              {invoice.totalAmount.toFixed(2)} €
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default InvoiceList;
