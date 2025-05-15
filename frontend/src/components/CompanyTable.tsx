import React, { useState } from 'react';
import type { ProcessedCompanyDto } from '../types';
import { TableHeader } from './table/TableHeader';
import { TableBody } from './table/TableBody';
import { Pagination } from './table/Pagination';

interface CompanyTableProps {
    companies: ProcessedCompanyDto[];
}

export const CompanyTable: React.FC<CompanyTableProps> = ({ companies }) => {
    const [currentPage, setCurrentPage] = useState(1);
    const [sortField, setSortField] = useState<keyof ProcessedCompanyDto>('firmaNavn');
    const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');
    const itemsPerPage =  50;

    const handleSort = (field: keyof ProcessedCompanyDto) => {
        if (field === sortField) {
            setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
        } else {
            setSortField(field);
            setSortDirection('asc');
        }
    };

    const handlePageChange = (page: number) => {
        setCurrentPage(page);
    };

    const sortedCompanies = [...companies].sort((a, b) => {
        const aValue = a[sortField] || '';
        const bValue = b[sortField] || '';

        if (typeof aValue === 'string' && typeof bValue === 'string') {
            return sortDirection === 'asc'
                ? aValue.localeCompare(bValue)
                : bValue.localeCompare(aValue);
        }

        // Number comparison
        const numA = Number(aValue);
        const numB = Number(bValue);
        return sortDirection === 'asc' ? numA - numB : numB - numA;
    });

    const startIndex = (currentPage - 1) * itemsPerPage;
    const paginatedCompanies = sortedCompanies.slice(startIndex, startIndex + itemsPerPage);
    const totalPages = Math.ceil(companies.length / itemsPerPage);

    return (
        <div className="mt-6">
            <h2 className="text-xl font-semibold mb-3">Company Data</h2>
            <div className="overflow-x-auto bg-dark-surface rounded-md">
                <table className="min-w-full divide-y divide-dark-border">
                    <TableHeader
                        sortField={sortField}
                        sortDirection={sortDirection}
                        onSort={handleSort}
                    />
                    <TableBody companies={paginatedCompanies}/>
                </table>
            </div>
            <Pagination
                currentPage={currentPage}
                totalPages={totalPages}
                onPageChange={handlePageChange}
            />
        </div>
    );
};