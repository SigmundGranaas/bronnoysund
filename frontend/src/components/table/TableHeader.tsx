import React from 'react';
import type {ProcessedCompanyDto} from '../../types';
import { SortIcon } from './SortIcon';

interface TableHeaderProps {
    sortField: keyof ProcessedCompanyDto;
    sortDirection: 'asc' | 'desc';
    onSort: (field: keyof ProcessedCompanyDto) => void;
}

export const TableHeader: React.FC<TableHeaderProps> = ({
                                                            sortField,
                                                            sortDirection,
                                                            onSort
                                                        }) => {
    const columns: Array<{
        key: keyof ProcessedCompanyDto;
        label: string;
    }> = [
        { key: 'orgNo', label: 'Org Number' },
        { key: 'firmaNavn', label: 'Company Name' },
        { key: 'status', label: 'Status' },
        { key: 'antallAnsatte', label: 'Employees' },
        { key: 'organisasjonsformKode', label: 'Org Form' },
        { key: 'naeringskode', label: 'Business Code' },
    ];

    return (
        <thead className="bg-dark-surface-hover">
        <tr>
            {columns.map(column => (
                <th
                    key={column.key}
                    className="px-4 py-3 text-left text-sm font-medium text-dark-text-primary tracking-wider cursor-pointer"
                    onClick={() => onSort(column.key)}
                >
                    <div className="flex items-center">
                        {column.label}
                        <SortIcon
                            active={sortField === column.key}
                            direction={sortField === column.key ? sortDirection : 'asc'}
                        />
                    </div>
                </th>
            ))}
        </tr>
        </thead>
    );
};
