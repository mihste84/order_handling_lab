import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { faAnglesLeft, faAnglesRight, faAngleRight, faAngleLeft } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-table-pagination',
  templateUrl: './table-pagination.component.html',
  styleUrls: ['./table-pagination.component.css'],
})
export class TablePaginationComponent {
  @Input({ required: true }) public page: number = 1;
  @Input({ required: true }) public totalItems: number = 0;
  @Input({ required: true }) public itemsPerPage: number = 0;
  @Output() public onPageChange = new EventEmitter<number>();
  public faAngleLeft = faAngleLeft;
  public faAngleRight = faAngleRight;
  public faAnglesLeft = faAnglesLeft;
  public faAnglesRight = faAnglesRight;

  public getTotalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  public getTotalPagesDisplay(): number {
    if (this.getTotalPages() === 0) return 1;
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  public onNextPage(): void {
    if (this.canSelectNextPage()) this.onPageChange.emit(this.page + 1);
  }

  public onPreviousPage(): void {
    if (this.page > 1) this.onPageChange.emit(this.page - 1);
  }

  public onLastPage(): void {
    const totalPages = this.getTotalPages();
    if (totalPages > this.page) this.onPageChange.emit(totalPages);
  }

  public onFirstPage(): void {
    if (this.page > 1 && this.page != 1) this.onPageChange.emit(1);
  }

  public canSelectNextPage(): boolean {
    const totalPages = this.getTotalPages();
    if (!totalPages) return false;
    return this.page >= 1 && this.page != totalPages;
  }
}
