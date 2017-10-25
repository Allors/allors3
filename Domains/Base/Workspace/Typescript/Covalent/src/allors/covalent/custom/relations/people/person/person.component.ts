import { AfterViewInit, Component, OnDestroy, OnInit } from "@angular/core";
import { Validators } from "@angular/forms";
import { Title } from "@angular/platform-browser";
import { ActivatedRoute } from "@angular/router";
import { TdDialogService, TdMediaService } from "@covalent/core";
import { Observable, Subject, Subscription } from "rxjs/Rx";

import { AllorsService, ErrorService, Loaded, Saved, Scope } from "@allors";
import { Equals, Fetch, Like, Page, Path, PullRequest, PushResponse, Query, Sort, TreeNode } from "@baseDomain";
import { Enumeration, Locale, Organisation, Person } from "../../../../../domain";
import { MetaDomain } from "../../../../../meta/index";

@Component({
  templateUrl: "./person.component.html",
})
export class PersonComponent implements OnInit, AfterViewInit, OnDestroy {

  private subscription: Subscription;
  private scope: Scope;

  title: string;

  m: MetaDomain;
  locales: Locale[];
  person: Person;

  constructor(
    private allorsService: AllorsService,
    private errorService: ErrorService,
    private titleService: Title,
    private route: ActivatedRoute,
    private media: TdMediaService) {

    this.title = "Person";
    this.titleService.setTitle(this.title);
    this.scope = new Scope(allorsService.database, allorsService.workspace);
    this.m = this.allorsService.meta;
  }

  ngOnInit(): void {
    this.subscription = this.route.url
      .switchMap((url: any) => {

        const id: string = this.route.snapshot.paramMap.get("id");

        const m: MetaDomain = this.allorsService.meta;

        const fetch: Fetch[] = [
          new Fetch({
            name: "person",
            id,
          }),
        ];

        const query: Query[] = [
          new Query(
            {
              name: "locales",
              objectType: this.m.Locale,
            }),
        ];

        this.scope.session.reset();

        return this.scope
          .load("Pull", new PullRequest({ fetch, query }));
      })
      .subscribe((loaded: Loaded) => {

        this.person = loaded.objects.person as Person;
        if (!this.person) {
          this.person = this.scope.session.create("Person") as Person;
        }

        this.locales = loaded.collections.locales as Locale[];
      },
      (error: any) => {
        this.errorService.message(error);
        this.goBack();
      },
    );
  }

  ngAfterViewInit(): void {
    this.media.broadcast();
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  save(): void {

    this.scope
      .save()
      .subscribe((saved: Saved) => {
        this.goBack();
      },
      (error: Error) => {
        this.errorService.dialog(error);
      });
  }

  goBack(): void {
    window.history.back();
  }
}
