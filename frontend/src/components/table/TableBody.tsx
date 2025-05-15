import React from 'react';
import type {ProcessedCompanyDto} from '../../types';
import { CompanyStatusBadge } from './CompanyStatusBadge';

interface TableBodyProps {
    companies: ProcessedCompanyDto[];
}

export const TableBody: React.FC<TableBodyProps> = ({ companies }) => {
    if (companies.length === 0) {
        return (
            <tbody>
            <tr>
                <td colSpan={6} className="px-4 py-8 text-center text-dark-text-secondary">
                    No company data available
                </td>
            </tr>
            </tbody>
        );
    }

    return (
        <tbody className="divide-y divide-dark-border">
        {companies.map((company, index) => (
            <tr
                key={company.orgNo || index}
                className="hover:bg-dark-surface-hover transition-colors"
            >
                <td className="px-4 py-3 text-sm">{company.orgNo || '-'}</td>
                <td className="px-4 py-3 text-sm font-medium">{company.firmaNavn || '-'}</td>
                <td className="px-4 py-3 text-sm">
                    <CompanyStatusBadge status={company.status} />
                </td>
                <td className="px-4 py-3 text-sm">{company.antallAnsatte || 0}</td>
                <td className="px-4 py-3 text-sm">{company.organisasjonsformKode || '-'}</td>
                <td className="px-4 py-3 text-sm">{company.naeringskode || '-'}</td>
            </tr>
        ))}
        </tbody>
    );
};
