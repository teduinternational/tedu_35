export class Category {
    id: number;
    name: string;
    seoAlias: string;
    seoDescription: string;
    sortOrder: number;
    parentId?: number;
    numberOfTickets: number;
}