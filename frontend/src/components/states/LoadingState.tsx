import React from 'react';

export const LoadingState: React.FC = () => {
    return (
        <div className="flex justify-center items-center h-64">
            <span className="ml-3 text-dark-text-secondary">Loading data...</span>
        </div>
    );
};
