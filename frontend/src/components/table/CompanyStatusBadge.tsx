import React from 'react';

interface CompanyStatusBadgeProps {
    status?: string;
}

export const CompanyStatusBadge: React.FC<CompanyStatusBadgeProps> = ({ status }) => {
    if (!status) return <span className="badge-default">-</span>;

    return status;
    
    // Didn't like this actually...
    switch(status.toUpperCase()) {
        case 'NORMAL':
            return (
                <span className="px-2 py-1 text-xs rounded-full bg-green-100 text-green-800">
                  Aktiv
                </span>
            );
        case 'SLETTET':
            return (
                <span className="px-2 py-1 text-xs rounded-full bg-red-100 text-red-800">
                  Slettet
                </span>
            );
        case 'KONKURS':
            return (
                <span className="px-2 py-1 text-xs rounded-full bg-orange-100 text-orange-800">
                  Konkurs
                </span>
            );
        case 'UnderAvvikling':
            return (
                <span className="px-2 py-1 text-xs rounded-full bg-orange-100 text-orange-800">
                  Under Avvikling
                </span>
            );
        default:
            return (
                <span className="px-2 py-1 text-xs rounded-full bg-gray-100 text-gray-800">
                  {status}
                </span>
            );
    }
};